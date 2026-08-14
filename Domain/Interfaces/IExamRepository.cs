using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IExamRepository : IGenericRepository<Exam>
    {
        Task<Exam> GetByTitleAsync(string title);
    }
}
