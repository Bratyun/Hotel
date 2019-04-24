using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IWorkWithDB<T>
    {
        T Create(T obj);
        T GetById(int id);
        List<T> GetAll();
        bool Delete(int id);
        T Update(int id, T obj);

    }
}
