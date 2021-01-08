using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Web.Contractors
{
    public interface IWebContractorServise
    {
        string UniqueCode { get; }
        string GetUrl { get; }

    }
}
