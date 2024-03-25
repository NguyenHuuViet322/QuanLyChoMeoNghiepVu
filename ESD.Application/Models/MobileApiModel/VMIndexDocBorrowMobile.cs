using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAS.Application.Models.ViewModels;

namespace DAS.Application.Models.MobileApiModel
{
    public class VMIndexDocBorrowMobile
    {
        public VMMobileCatalogingProfile VmCatalogingProfile { get; set; }
        public PaginatedList<VMMobileCatalogingDoc> VMMobileCatalogingDocs { get; set; }
        public PaginatedList<VMMobileCatalogingBorrowDoc> WaitingCatalogingBorrowByReader { get; set; }
        public int TotalDocs { get; set; }
    }
}
