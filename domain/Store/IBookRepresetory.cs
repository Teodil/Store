using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store
{
    public interface IBookRepresetory
    {
        Book[] GetAllByTitle(string titlePart);
    }
}
