using LatexRendererAPI.Data;
using LatexRendererAPI.Models.Domain;
using LatexRendererAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LatexRendererAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private AppDbContext dbContext;

        public ProjectController(AppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public IActionResult GetProjectById([FromRoute] Guid id)
        {
            var project = dbContext.Projects.Find(id);
            if (project == null)
            {
                return NotFound();
            }
            return Ok(project);
        }

        [HttpGet]
        public IActionResult GetListProjects([FromQuery] GetListProjectsQueryDto query)
        {
            if (ModelState.IsValid)
            {
                var projects = dbContext
                    .Projects.Include(p => p.UserProjects)
                    .Include(p => p.StarProjects)
                    .Include(p => p.Versions)
                    .AsQueryable();

                var currentUser = HttpContext.User;
                var userId = Guid.Parse(User.Claims.First(claim => claim.Type == "UserId").Value);

                switch (query.Category)
                {
                    case "all":
                        projects = projects.Where(p =>
                            p.UserProjects.Any(up => up.EditorId == userId)
                        );
                        break;
                    case "yours":
                        projects = projects.Where(p =>
                            p.UserProjects.Any(up => up.EditorId == userId && up.Role == "owner")
                        );
                        break;
                    case "shared":
                        projects = projects.Where(p =>
                            p.UserProjects.Any(up => up.EditorId == userId && up.Role != "owner")
                        );
                        break;
                    case "starred":
                        projects = projects.Where(p =>
                            p.StarProjects.Any(up => up.EditorId == userId)
                        );
                        break;
                }

                if (!string.IsNullOrWhiteSpace(query.Keyword))
                    projects = projects.Where(e => e.Name.Contains(query.Keyword));

                if (query.FieldSort != null && query.Sort != null)
                {
                    if (query.FieldSort == "name")
                        if (query.Sort.Equals("ascend"))
                            projects = projects.Distinct().OrderBy(x => x.Name);
                        else
                            projects = projects.Distinct().OrderByDescending(x => x.Name);
                    else if (query.FieldSort == "modifiedTime")
                    {
                        if (query.Sort.Equals("ascend"))
                            projects = projects.Distinct().OrderBy(p =>
                                p.Versions.Where(v => v.IsMainVersion)
                                    .Select(v => v.ModifiedTime)
                                    .FirstOrDefault()
                            );
                        else
                            projects = projects.Distinct().OrderByDescending(p =>
                                p.Versions.Where(v => v.IsMainVersion)
                                    .Select(v => v.ModifiedTime)
                                    .FirstOrDefault()
                            );
                    }
                }
                var skipResults = (query.Page - 1) * query.PageSize;
                return Ok(
                    new
                    {
                        list = projects
                            .Skip(skipResults)
                            .Take(query.PageSize)   
                            .Select(p => new
                            {
                                p.Id,
                                p.Name,
                                p.IsPublic,
                                p.Versions,
                                MainVersion = p
                                    .Versions.Where(v => v.IsMainVersion)
                                    .AsQueryable()
                                    .Include(v => v.Editor)
                                    .Select(v => new
                                    {
                                        v.Id,
                                        v.ModifiedTime,
                                        v.Editor
                                    })
                                    .First(),
                                UserProjects = p.UserProjects.Select(up => new
                                {
                                    up.Editor.Fullname,
                                    up.Editor.Username,
                                }),
                                p.UserProjects.First(up => up.EditorId == userId).Role,
                                Starred = p.StarProjects.Any(up => up.EditorId == userId)
                                    ? false
                                    : true,
                            })
                            .ToList(),
                        total = projects.Count(),
                    }
                );
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        public IActionResult CreateProject(
            [FromBody] CreateProjectRequestDto createProjectRequestDto
        )
        {
            var currentUser = HttpContext.User;
            var userId = User.Claims.First(claim => claim.Type == "UserId").Value;

            var newProject = new ProjectModel
            {
                Name = createProjectRequestDto.Name,
                MainVersionId = new Guid()
            };
            dbContext.Projects.Add(newProject);

            var newUserProject = new UserProject
            {
                ProjectId = newProject.Id,
                EditorId = Guid.Parse(userId),
                Role = "owner"
            };
            dbContext.UserProjects.Add(newUserProject);

            var newVersion = new VersionModel
            {
                EditorId = Guid.Parse(userId),
                ProjectId = newProject.Id,
                ModifiedTime = DateTime.Now,
                IsMainVersion = true,
                MainFileId = new Guid(),
                Description = "Main Version"
            };
            dbContext.Versions.Add(newVersion);

            newProject.MainVersionId = newVersion.Id;

            var mainFile = new FileModel
            {
                Name = "main.tex",
                Content = "",
                Path = "main.tex",
                Type = "tex",
                VersionId = newVersion.Id
            };
            dbContext.Files.Add(mainFile);

            newVersion.MainFileId = mainFile.Id;

            dbContext.SaveChanges();
            // return Ok(newProject);
            return Ok();
        }

        [HttpPost]
        [Route("copyProject")]
        public IActionResult CopyProject([FromBody] CopyProjectDto dto)
        {
            var currentUser = HttpContext.User;
            var userId = User.Claims.First(claim => claim.Type == "UserId").Value;

            var copyVersion = dbContext.Versions.Find(dto.VersionId);
            if (copyVersion == null)
                return NotFound();
            var mainCopyFile = dbContext.Files.Find(copyVersion.MainFileId);

            var project = new ProjectModel { Name = dto.Name, MainVersionId = new Guid() };
            dbContext.Add(project);

            var userProject = new UserProject
            {
                EditorId = Guid.Parse(userId),
                ProjectId = project.Id,
                Role = "owner"
            };
            dbContext.Add(userProject);

            var version = new VersionModel
            {
                ProjectId = project.Id,
                ModifiedTime = DateTime.Now,
                EditorId = Guid.Parse(userId),
                IsMainVersion = true,
                Description = "Main version",
                MainFileId = new Guid()
            };
            dbContext.Add(version);

            project.MainVersionId = version.Id;

            var files = dbContext.Files.Where(f => f.VersionId == dto.VersionId);

            Parallel.ForEach(
                files,
                f =>
                {
                    var newFile = new FileModel
                    {
                        Content = f.Content,
                        Type = f.Type,
                        Path = f.Path,
                        Name = f.Name,
                        VersionId = version.Id
                    };
                    dbContext.Files.Add(newFile);

                    if (mainCopyFile != null && newFile.Path == mainCopyFile.Path)
                    {
                        version.MainFileId = newFile.Id;
                    }
                }
            );
            // var mainFile = dbContext.Files.First(f => f.Path == mainCopyFile.Path);
            // if (mainFile != null)
            //     version.MainFileId = mainFile.Id;
            dbContext.SaveChanges();
            return Ok();
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteProject([FromRoute] Guid id)
        {
            var project = await dbContext.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            dbContext.Projects.Remove(project);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public IActionResult UpdateProject([FromRoute] Guid id, [FromBody] UpdateProjectDto dto)
        {
            var project = dbContext.Projects.Find(id);
            if (project == null)
            {
                return NotFound();
            }
            if (dto.Name != null && dto.Name != "")
                project.Name = dto.Name;
            if (dto.IsPublic == true)
                project.IsPublic = true;
            if (dto.IsPublic == false)
                project.IsPublic = false;

            dbContext.SaveChanges();

            return Ok(dto);
        }
    }
}
