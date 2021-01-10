using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Web.Contractors
{
    public interface IWebContractorService
    {
        string Name { get; }
        Uri StartSession(IReadOnlyDictionary<string, string> parameter, Uri returnUri);

    }
}
