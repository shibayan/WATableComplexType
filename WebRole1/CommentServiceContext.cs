using System.Linq;

using Microsoft.WindowsAzure;

using WebRole1.Models;

namespace WebRole1
{
    public class CommentServiceContext : ComplexTypeTableServiceContext
    {
        public CommentServiceContext(string baseAddress, StorageCredentials credentials)
            : base(baseAddress, credentials)
        {
        }

        public void AddObject<T>(T entity)
        {
            AddObject(typeof(T).Name, entity);
        }

        public IQueryable<Comment> Comment
        {
            get { return CreateQuery<Comment>("Comment"); }
        }
    }
}