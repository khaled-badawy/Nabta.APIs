using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Nabta_Production.DAL;
using Nabta_Production.DAL.Data;

namespace Nabta_Production.BL
{
    public class PostsManager : IPostsManager
    {
        private readonly IPostsRepo _postsRepo;
        private readonly ClimateConfNewContext _context;
        private readonly IConfiguration _config;
        private string url { get; }
        private string urlUserPicture { get; }
        private string urlUserDefaultPicture { get; }
        private string urlProjectIcon { get; }
        private string urlShareLink { get; }
        public PostsManager(IPostsRepo postsRepo , ClimateConfNewContext context , IConfiguration config)
        {
            _postsRepo = postsRepo;
            _context = context;
            _config = config;
            url = $"{_config.GetSection("ServerDownloadPath").Value!}/posts/attachments";
            urlProjectIcon = $"{_config.GetSection("ServerDownloadPath").Value!}/projects";
            urlUserPicture = $"{_config.GetSection("ServerDownloadPath").Value!}/users/attachments";
            urlUserDefaultPicture = $"{_config.GetSection("ServerDownloadPath").Value!}/default.png";
            urlShareLink = $"{_config.GetSection("ShareLink").Value!}/posts/details";
        }

        public List<ReadPostsDto> GetAllPosts(int pageNumber, string? search)
        {
            if (String.IsNullOrEmpty(search))
            {
                try
                {
                    var postsFromDB = _context.ClmPosts
                        .Where(p => p.Active == true)
                        .Include(p => p.User)
                        .Include(p => p.Type)
                        .OrderByDescending(p => p.CreateDate)
                        .GroupJoin(_context.ClmAttachments
                        .Where(a => a.ParentCategoryId == 1 && a.Active == 1),
                        post => post.Id,
                        attachment => attachment.ParentId,
                        (post, attachment) => new
                        {
                            Post = post,
                            Attachment = attachment.Select(a => a.FileName).ToList(),
                        })
                        .Skip((pageNumber - 1) * 10)
                        .Take(10)
                        .ToList();

                    //using (StreamWriter writer = System.IO.File.AppendText($"\\\\kitkat\\idscroot2341\\Tawasol\\NabtaAPI\\wwwroot\\upload\\errors.txt"))
                    //{
                    //    writer.WriteLine(" \n" + $"{postsFromDB.Count.ToString()}");
                    //}

                    if (postsFromDB.IsNullOrEmpty()) return new List<ReadPostsDto>();
                    return postsFromDB
                        .Select(p => new ReadPostsDto
                        {
                            Id = p.Post.Id,
                            Title = p.Post.TitleA,
                            NoOfComments = p.Post.NoOfComments,
                            NoOfLikes = p.Post.NoOfLikes,
                            NoOfViews = p.Post.NoOfViews,
                            TypeId = p.Post.TypeId,
                            TypeName = p.Post.Type.NameA,
                            UserID = p.Post.User != null ? p.Post.UserId : null,
                            UserName = p.Post.User != null ? p.Post.User.FullName : null,
                            UserPicture = p.Post.User == null ? urlUserDefaultPicture : p.Post.User.ProfilePicture == null ? null : p.Post.User.OldId == null ? $"{urlUserPicture}/{p.Post.User.Id}/{p.Post.User.ProfilePicture}" : $"{urlUserPicture}/{p.Post.User.OldId}/{p.Post.User.ProfilePicture}",
                            PuplishedDate = (DateTime.Now - p.Post.CreateDate).TotalDays,
                            Attachments = p.Attachment.Select(a => $"{url}/{p.Post.Id}/{a}").ToList(),
                            ShareLink = $"{urlShareLink}/{p.Post.Id}"
                        }).ToList();

                }
                catch (Exception ex)
                {
                    using (StreamWriter writer = System.IO.File.AppendText($"\\\\kitkat\\idscroot2341\\Tawasol\\NabtaAPI\\wwwroot\\upload\\errors.txt"))
                    {
                        writer.WriteLine(" \n" + $"{ex.Message}");
                    }
                    throw;
                }
               
            }

            try
            {
                var postsWithSearchFromDB = _context.ClmPosts
                      .Where(p => p.Active == true &&
                      (p.TitleA.Contains(search!) || p.DescriptionA.Contains(search!)))
                      .Include(p => p.User)
                      .Include(p => p.Type)
                      .OrderByDescending(p => p.CreateDate)
                      .GroupJoin(_context.ClmAttachments
                      .Where(a => a.ParentCategoryId == 1 && a.Active == 1),
                      post => post.Id,
                      attachment => attachment.ParentId,
                      (post, attachment) => new
                      {
                          Post = post,
                          Attachment = attachment.Select(a => a.FileName).ToList(),
                      })
                      .Skip((pageNumber - 1) * 10)
                      .Take(10)
                      .ToList();

                //using (StreamWriter writer = System.IO.File.AppendText($"\\\\kitkat\\idscroot2341\\Tawasol\\NabtaAPI\\wwwroot\\upload\\errors.txt"))
                //{
                //    writer.WriteLine(" \n" + $"{postsWithSearchFromDB.Count.
                //        ToString()}");
                //}

                if (postsWithSearchFromDB.Count == 0) return new List<ReadPostsDto>();

                return postsWithSearchFromDB.Select(p => new ReadPostsDto
                {
                    Id = p.Post.Id,
                    Title = p.Post.TitleA,
                    NoOfComments = p.Post.NoOfComments,
                    NoOfLikes = p.Post.NoOfLikes,
                    NoOfViews = p.Post.NoOfViews,
                    TypeId = p.Post.TypeId,
                    TypeName = p.Post.Type.NameA,
                    UserID = p.Post.User != null ? p.Post.UserId : null,
                    UserName = p.Post.User != null ? p.Post.User.FullName : null,
                    UserPicture = p.Post.User == null ?  urlUserDefaultPicture : p.Post.User.ProfilePicture == null ? null : p.Post.User.OldId == null ? $"{urlUserPicture}/{p.Post.User.Id}/{p.Post.User.ProfilePicture}" : $"{urlUserPicture}/{p.Post.User.OldId}/{p.Post.User.ProfilePicture}",
                    PuplishedDate = (DateTime.Now - p.Post.CreateDate).TotalDays,
                    Attachments = p.Attachment.Select(a => $"{url}/{p.Post.Id}/{a}").ToList(),
                    ShareLink = $"{urlShareLink}/{p.Post.Id}"
                }).ToList();

            }
            catch (Exception ex)
            {
                using (StreamWriter writer = System.IO.File.AppendText($"\\\\kitkat\\idscroot2341\\Tawasol\\NabtaAPI\\wwwroot\\upload\\errors.txt"))
                {
                    writer.WriteLine(" \n" + $"{ex.Message}");
                }
                throw;
            }
        }

        public List<ReadIdeasDto> GetIdeas(int pageNumber, string? search)
        {
            if (String.IsNullOrEmpty(search))
                {
                return _context.ClmPosts
                    .Where(p => p.Active == true && p.ProjectId == null && p.UserId != null)
                    .Include(p => p.User)
                    .OrderByDescending(p => p.CreateDate)
                    .GroupJoin(_context.ClmAttachments
                    .Where(a => a.ParentCategoryId == 1 && a.Active == 1),
                    post => post.Id,
                    attachment => attachment.ParentId,
                    (post, attachment) => new
                    {
                        Post = post,
                        Attachment = attachment.Select(a=> a.FileName).ToList(),
                    }).Skip((pageNumber - 1 )* 10)
                    .Take(10)
                    .Select(p => new ReadIdeasDto
                    {
                        Id = p.Post.Id,
                        Title = p.Post.TitleA,
                        Description = p.Post.DescriptionA,
                        NoOfComments = p.Post.NoOfComments,
                        NoOfLikes = p.Post.NoOfLikes,
                        NoOfViews = p.Post.NoOfViews,
                        UserID = p.Post.User != null ? p.Post.UserId : null,
                        UserName = p.Post.User != null ? p.Post.User.FullName : null,
                        UserPicture = p.Post.User == null ? urlUserDefaultPicture : p.Post.User.ProfilePicture == null ? null : p.Post.User.OldId == null ? $"{urlUserPicture}/{p.Post.User.Id}/{p.Post.User.ProfilePicture}" : $"{urlUserPicture}/{p.Post.User.OldId}/{p.Post.User.ProfilePicture}",
                        PuplishedDate = (DateTime.Now - p.Post.CreateDate).TotalDays,
                        Attachments = p.Attachment.Select(a=> $"{url}/{p.Post.Id}/{a}").ToList(),
                        ShareLink = $"{urlShareLink}/{p.Post.Id}"
                    }).ToList();
            }
            return _context.ClmPosts
                  .Where(p => p.Active == true && p.ProjectId == null && p.UserId != null && (p.TitleA.Contains(search!) || p.DescriptionA.Contains(search!)))
                  .Include(p => p.User)
                  .OrderByDescending(p => p.CreateDate)
                  .GroupJoin(_context.ClmAttachments
                  .Where(a => a.ParentCategoryId == 1 && a.Active == 1),
                  post => post.Id,
                  attachment => attachment.ParentId,
                  (post, attachment) => new
                  {
                      Post = post,
                      Attachment = attachment.Select(a => a.FileName).ToList(),
                  }).Skip((pageNumber - 1) * 10)
                  .Take(10)
                  .Select(p => new ReadIdeasDto
                  {
                      Id = p.Post.Id,
                      Title = p.Post.TitleA,
                      Description = p.Post.DescriptionA,
                      NoOfComments = p.Post.NoOfComments,
                      NoOfLikes = p.Post.NoOfLikes,
                      NoOfViews = p.Post.NoOfViews,
                      UserID = p.Post.User != null ? p.Post.UserId : null,
                      UserName = p.Post.User != null ? p.Post.User.FullName : null,
                      UserPicture = p.Post.User == null ? urlUserDefaultPicture : p.Post.User.ProfilePicture == null ? null : p.Post.User.OldId == null ? $"{urlUserPicture}/{p.Post.User.Id}/{p.Post.User.ProfilePicture}" : $"{urlUserPicture}/{p.Post.User.OldId}/{p.Post.User.ProfilePicture}",
                      PuplishedDate = (DateTime.Now - p.Post.CreateDate).TotalDays,
                      Attachments = p.Attachment.Select(a => $"{url}/{p.Post.Id}/{a}").ToList(),
                      ShareLink = $"{urlShareLink}/{p.Post.Id}"
                  }).ToList();
        }

        public List<ReadNewsDto> GetNews(int pageNumber , string? search)
        {
            if (String.IsNullOrEmpty(search))
            {
                
            return _context.ClmPosts
                .Where(p => p.Active == true && p.UserId == null)
                .OrderByDescending(p => p.CreateDate)
                .GroupJoin(_context.ClmAttachments
                .Where(a => a.ParentCategoryId == 1 && a.Active == 1),
                post => post.Id,
                attachment => attachment.ParentId,
                (post, attachment) => new
                {
                    Post = post,
                    Attachment = attachment.Select(a => a.FileName).ToList(),
                })
                .Skip((pageNumber - 1) * 10)
                .Take(10)
                .Select(p => new ReadNewsDto
                {
                    Id = p.Post.Id,
                    Title = p.Post.TitleA,
                    Description = p.Post.DescriptionA,
                    NoOfComments = p.Post.NoOfComments,
                    NoOfLikes = p.Post.NoOfLikes,
                    NoOfViews = p.Post.NoOfViews,
                    PuplishedDate = (DateTime.Now - p.Post.CreateDate).TotalDays,
                    Attachments = p.Attachment.Select(a => $"{url}/{p.Post.Id}/{a}").ToList(),
                    ShareLink = $"{urlShareLink}/{p.Post.Id}",
                    UserPicture = urlUserDefaultPicture
                }).ToList();
            }

            return _context.ClmPosts
               .Where(p => p.Active == true && p.UserId == null && (p.TitleA.Contains(search!) || p.DescriptionA.Contains(search!)))
               .OrderByDescending(p => p.CreateDate)
               .GroupJoin(_context.ClmAttachments
               .Where(a => a.ParentCategoryId == 1 && a.Active == 1),
               post => post.Id,
               attachment => attachment.ParentId,
               (post, attachment) => new
               {
                   Post = post,
                   Attachment = attachment.Select(a => a.FileName).ToList(),
               })
               .Skip((pageNumber - 1) * 10)
               .Take(10)
               .Select(p => new ReadNewsDto
               {
                   Id = p.Post.Id,
                   Title = p.Post.TitleA,
                   Description = p.Post.DescriptionA,
                   NoOfComments = p.Post.NoOfComments,
                   NoOfLikes = p.Post.NoOfLikes,
                   NoOfViews = p.Post.NoOfViews,
                   PuplishedDate = (DateTime.Now - p.Post.CreateDate).TotalDays,
                   Attachments = p.Attachment.Select(a => $"{url}/{p.Post.Id}/{a}").ToList(),
                   ShareLink = $"{urlShareLink}/{p.Post.Id}",
                   UserPicture = urlUserDefaultPicture
               }).ToList();
        }

        public ReadPostByIdDto? GetPostById(int postID)
        {
            var postFromDB = _context.ClmPosts
                .Include(p => p.Project)
                .Include(p => p.User)
                .Include(p => p.ClmComments).ThenInclude(p => p.User)
                .Include(p => p.Type)
                .GroupJoin(_context.ClmAttachments
                .Where(a => a.ParentCategoryId == 1 && a.Active == 1),
                post => post.Id,
                attachment => attachment.ParentId,
                (post, attachment) => new
                {
                    Post = post,
                    Attachment = attachment.Select(a => a.FileName).ToList(),
                })
                .FirstOrDefault(p => p.Post.Id == postID && p.Post.Active == true);

            if (postFromDB == null) return null;

            postFromDB.Post.NoOfViews += 1;
            _postsRepo.SaveChanges();

            var postWithProject = new ReadPostByIdDto
            {
                Id = postFromDB.Post.Id,
                Title = postFromDB.Post.TitleA,
                Description = postFromDB.Post.DescriptionA,
                NoOfComments = postFromDB.Post.NoOfComments,
                NoOfLikes = postFromDB.Post.NoOfLikes,
                NoOfViews = postFromDB.Post.NoOfViews,
                UserID = postFromDB.Post.User != null ? postFromDB.Post.UserId : null,
                UserName = postFromDB.Post.User != null ? postFromDB.Post.User.FullName : null,
                UserPicture = postFromDB.Post.User == null ? urlUserDefaultPicture : postFromDB.Post.User.ProfilePicture == null ? null : postFromDB.Post.User.OldId == null ? $"{urlUserPicture}/{postFromDB.Post.User.Id}/{postFromDB.Post.User.ProfilePicture}" : $"{urlUserPicture}/{postFromDB.Post.User.OldId}/{postFromDB.Post.User.ProfilePicture}",
                TypeId = postFromDB.Post.TypeId,
                TypeName = postFromDB.Post.Type.NameA,
                ProjectID = postFromDB.Post.ProjectId,
                ProjectName = postFromDB.Post.Project != null ? postFromDB.Post.Project.NameA : null,
                ProjectIcon = postFromDB.Post.Project != null ? $"{urlProjectIcon}/{postFromDB.Post.ProjectId}/{postFromDB.Post.Project?.Icon}" : null,
                PuplishedDate = (DateTime.Now - postFromDB.Post.CreateDate).TotalDays,
                SourceLink = postFromDB.Post.SourceLink,
                Attachments = postFromDB.Attachment.Select(a => $"{url}/{postFromDB.Post.Id}/{a}").ToList(),
                ShareLink = $"{urlShareLink}/{postFromDB.Post.Id}"
            };

            postWithProject.Comments = postFromDB.Post.ClmComments
                .Where(c => c.Active == true )
                .Select(c => new ReadCommentsDto
            {
                Id = c.Id,
                Comment = c.Comment,
                UserId = c.User.Id,
                UserName = c.User.FullName!,
                UserPicture = c.User.ProfilePicture == null ? null : c.User.OldId == null ? $"{urlUserPicture}/{c.User.Id}/{c.User.ProfilePicture}" : $"{urlUserPicture}/{c.User.OldId}/{c.User.ProfilePicture}",
                CommentDate = (DateTime.Now - c.CreateDate).TotalDays
            }).ToList();

            return postWithProject;
        }

        public List<ReadPostsDto> GetProjects(int pageNumber, string? search)
        {
            if (String.IsNullOrEmpty(search))
            {
            return _context.ClmPosts
                .Where(p => p.Active == true && p.ProjectId != null)
                .Include(p => p.User)
                .Include(p => p.Type)
                .OrderByDescending(p => p.CreateDate)
                .GroupJoin(_context.ClmAttachments
                .Where(a => a.ParentCategoryId == 1 && a.Active == 1),
                post => post.Id,
                attachment => attachment.ParentId,
                (post, attachment) => new
                {
                    Post = post,
                    Attachment = attachment.Select(a => a.FileName).ToList(),
                }).Skip((pageNumber - 1) * 10)
                .Take(10)
                .Select(p => new ReadPostsDto
                {
                    Id = p.Post.Id,
                    Title = p.Post.TitleA,
                    NoOfComments = p.Post.NoOfComments,
                    NoOfLikes = p.Post.NoOfLikes,
                    NoOfViews = p.Post.NoOfViews,
                    TypeId = p.Post.TypeId,
                    TypeName = p.Post.Type.NameA,
                    UserID = p.Post.UserId,
                    UserName = p.Post.User!.FullName,
                    UserPicture = p.Post.User.ProfilePicture == null ? null : p.Post.User.OldId == null ? $"{urlUserPicture}/{p.Post.User.Id}/{p.Post.User.ProfilePicture}" : $"{urlUserPicture}/{p.Post.User.OldId}/{p.Post.User.ProfilePicture}",
                    PuplishedDate = (DateTime.Now - p.Post.CreateDate).TotalDays,
                    Attachments = p.Attachment.Select(a => $"{url}/{p.Post.Id}/{a}").ToList(),
                    ShareLink = $"{urlShareLink}/{p.Post.Id}"
                }).ToList();
            }

            return _context.ClmPosts
              .Where(p => p.Active == true && p.ProjectId != null && (p.TitleA.Contains(search!) || p.DescriptionA.Contains(search!)))
              .Include(p => p.User)
              .Include(p => p.Type)
              .OrderByDescending(p => p.CreateDate)
              .GroupJoin(_context.ClmAttachments
              .Where(a => a.ParentCategoryId == 1 && a.Active == 1),
              post => post.Id,
              attachment => attachment.ParentId,
              (post, attachment) => new
              {
                  Post = post,
                  Attachment = attachment.Select(a => a.FileName).ToList(),
              }).Skip((pageNumber - 1) * 10)
              .Take(10)
              .Select(p => new ReadPostsDto
              {
                  Id = p.Post.Id,
                  Title = p.Post.TitleA,
                  NoOfComments = p.Post.NoOfComments,
                  NoOfLikes = p.Post.NoOfLikes,
                  NoOfViews = p.Post.NoOfViews,
                  TypeId = p.Post.TypeId,
                  TypeName = p.Post.Type.NameA,
                  UserID = p.Post.UserId,
                  UserName = p.Post.User!.FullName,
                  UserPicture = p.Post.User.ProfilePicture == null ? null : p.Post.User.OldId == null ? $"{urlUserPicture}/{p.Post.User.Id}/{p.Post.User.ProfilePicture}" : $"{urlUserPicture}/{p.Post.User.OldId}/{p.Post.User.ProfilePicture}",
                  PuplishedDate = (DateTime.Now - p.Post.CreateDate).TotalDays,
                  Attachments = p.Attachment.Select(a => $"{url}/{p.Post.Id}/{a}").ToList(),
                  ShareLink = $"{urlShareLink}/{p.Post.Id}"
              }).ToList();

        }

        public List<ReadPostsDto> GetYourShares(int pageNumber , string? search)
        {
            if (String.IsNullOrEmpty(search))
            {
            return _context.ClmPosts
               .Where(p => p.Active == true && ((p.ProjectId == null && p.UserId != null) || (p.ProjectId != null)))
               .Include(p => p.User)
               .Include(p => p.Type)
               .OrderByDescending(p => p.CreateDate)
               .GroupJoin(_context.ClmAttachments
               .Where(a => a.ParentCategoryId == 1 && a.Active == 1),
               post => post.Id,
               attachment => attachment.ParentId,
               (post, attachment) => new
               {
                   Post = post,
                   Attachment = attachment.Select(a => a.FileName).ToList(),
               })
               .Skip((pageNumber - 1) * 10)
               .Take(10)
               .Select(p => new ReadPostsDto
               {
                   Id = p.Post.Id,
                   Title = p.Post.TitleA,
                   NoOfComments = p.Post.NoOfComments,
                   NoOfLikes = p.Post.NoOfLikes,
                   NoOfViews = p.Post.NoOfViews,
                   TypeId = p.Post.TypeId,
                   TypeName = p.Post.Type.NameA,
                   UserID = p.Post.UserId,
                   UserName = p.Post.User!.FullName,
                   UserPicture = p.Post.User.ProfilePicture == null ? null : p.Post.User.OldId == null ? $"{urlUserPicture}/{p.Post.User.Id}/{p.Post.User.ProfilePicture}" : $"{urlUserPicture}/{p.Post.User.OldId}/{p.Post.User.ProfilePicture}",
                   PuplishedDate = (DateTime.Now - p.Post.CreateDate).TotalDays,
                   Attachments = p.Attachment.Select(a => $"{url}/{p.Post.Id}/{a}").ToList(),
                   ShareLink = $"{urlShareLink}/{p.Post.Id}"
               }).ToList();
            }

            return _context.ClmPosts
              .Where(p => p.Active == true && ((p.ProjectId == null && p.UserId != null) || (p.ProjectId != null)) && (p.TitleA.Contains(search!) || p.DescriptionA.Contains(search!)))
              .Include(p => p.User)
              .Include(p => p.Type)
              .OrderByDescending(p => p.CreateDate)
              .GroupJoin(_context.ClmAttachments
              .Where(a => a.ParentCategoryId == 1 && a.Active == 1),
              post => post.Id,
              attachment => attachment.ParentId,
              (post, attachment) => new
              {
                  Post = post,
                  Attachment = attachment.Select(a => a.FileName).ToList(),
              })
              .Skip((pageNumber - 1) * 10)
              .Take(10)
              .Select(p => new ReadPostsDto
              {
                  Id = p.Post.Id,
                  Title = p.Post.TitleA,
                  NoOfComments = p.Post.NoOfComments,
                  NoOfLikes = p.Post.NoOfLikes,
                  NoOfViews = p.Post.NoOfViews,
                  TypeId = p.Post.TypeId,
                  TypeName = p.Post.Type.NameA,
                  UserID = p.Post.UserId,
                  UserName = p.Post.User!.FullName,
                  UserPicture = p.Post.User.ProfilePicture == null ? null : p.Post.User.OldId == null ? $"{urlUserPicture}/{p.Post.User.Id}/{p.Post.User.ProfilePicture}" : $"{urlUserPicture}/{p.Post.User.OldId}/{p.Post.User.ProfilePicture}",
                  PuplishedDate = (DateTime.Now - p.Post.CreateDate).TotalDays,
                  Attachments = p.Attachment.Select(a => $"{url}/{p.Post.Id}/{a}").ToList(),
                  ShareLink = $"{urlShareLink}/{p.Post.Id}"
              }).ToList();

        }

        public async Task<int> Add(AddPostDto post, int userId)
        {
            ClmPost postToAdd = new ClmPost()
            {
                OwnerId = userId,
                CreateDate = DateTime.Now,
                TitleA = post.Title,
                DescriptionA = post.Description,
                TypeId = post.TypeId,
                ProjectId = post.ProjectId,
                UserId = userId,
                Active = false,
                SortIndex = 0,
                Focus = 0,
                NoOfLikes = 0,
                NoOfComments = 0,
                NoOfViews = 0,
            };

            await _postsRepo.Add(postToAdd);

            _postsRepo.SaveChanges();
            
            return postToAdd.Id;
        }

        public bool LikePost(int postId)
        {
            ClmPost? post = _postsRepo.GetPostById(postId);
            if (post == null) return false;
            post.NoOfLikes += 1;
            _postsRepo.SaveChanges();
            return true;
        }

        public bool UnLikePost(int postId)
        {
            ClmPost? post = _postsRepo.GetPostById(postId);
            if (post == null) return false;
            if (post.NoOfLikes > 0)
            {
                post.NoOfLikes -= 1;
            }
            _postsRepo.SaveChanges();
            return true;
        }
        public List<ReadPostsDto> GetMyShares(int pageNumber, int userId)
        {
            return _context.ClmPosts
              .Where(p => p.Active == true && p.UserId == userId)
              .Include(p => p.User)
              .Include(p => p.Type)
              .OrderByDescending(p => p.CreateDate)
              .GroupJoin(_context.ClmAttachments
              .Where(a => a.ParentCategoryId == 1 && a.Active == 1),
              post => post.Id,
              attachment => attachment.ParentId,
              (post, attachment) => new
              {
                  Post = post,
                  Attachment = attachment.Select(a => a.FileName).ToList(),
              })
              .Skip((pageNumber - 1) * 10)
              .Take(10)
              .Select(p => new ReadPostsDto
              {
                  Id = p.Post.Id,
                  Title = p.Post.TitleA,
                  NoOfComments = p.Post.NoOfComments,
                  NoOfLikes = p.Post.NoOfLikes,
                  NoOfViews = p.Post.NoOfViews,
                  TypeId = p.Post.TypeId,
                  TypeName = p.Post.Type.NameA,
                  UserID = p.Post.UserId,
                  UserName = p.Post.User!.FullName,
                  UserPicture = p.Post.User.ProfilePicture == null ? null : p.Post.User.OldId == null ? $"{urlUserPicture}/{p.Post.User.Id}/{p.Post.User.ProfilePicture}" : $"{urlUserPicture}/{p.Post.User.OldId}/{p.Post.User.ProfilePicture}",
                  PuplishedDate = (DateTime.Now - p.Post.CreateDate).TotalDays,
                  Attachments = p.Attachment.Select(a => $"{url}/{p.Post.Id}/{a}").ToList(),
                  ShareLink = $"{urlShareLink}/{p.Post.Id}"
              }).ToList();
        }
        public bool DeleteUserPost(int userId, int postId)
        {
            var post = _postsRepo.GetPostForUserById(userId, postId);
            if (post != null)
            {
                _postsRepo.Delete(post);
                return true;
            }
            return false;
        }
    }
}
