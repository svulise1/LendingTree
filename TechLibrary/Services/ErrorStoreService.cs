using System;
using System.Threading.Tasks;
using TechLibrary.Data;
using TechLibrary.Data.Entities;

namespace TechLibrary.Services
{

    public interface IErrorStoreService
    {
        Task RecordException(Exception ex, string body);
    }
    public class ErrorStoreService : IErrorStoreService
    {
        private readonly DataContext _dataContext;

        public ErrorStoreService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }


        /// <summary>
        /// Add exception to error store table, so developer can revisit them
        /// </summary>
        /// <returns></returns>
        public async Task RecordException(Exception ex, string body)
        {
            ErrorStore errorStore = new ErrorStore {StackTrace = ex.StackTrace, Body = body, ErrorMessage = ex.Message};
            await _dataContext.ErrorStore.AddAsync(errorStore);
            await _dataContext.SaveChangesAsync();

        }
    }
}