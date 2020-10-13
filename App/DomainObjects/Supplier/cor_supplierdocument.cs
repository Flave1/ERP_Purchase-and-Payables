using Puchase_and_payables.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GODP.APIsContinuation.DomainObjects.Supplier
{
    public partial class cor_supplierdocument : GeneralEntity
    {
        [Key]
        public int SupplierDocumentId { get; set; }

        public int SupplierId { get; set; }
         
        public int DocumentId { get; set; }

        [Column(TypeName = "image")]
        public byte[] Document { get; set; }
        public string FileType { get; set; }
        public string Extension { get; set; }
        public string ReferenceNumber { get; set; }
        public string Description { get; set; }

        public virtual cor_supplier cor_supplier { get; set; }
    }
}
