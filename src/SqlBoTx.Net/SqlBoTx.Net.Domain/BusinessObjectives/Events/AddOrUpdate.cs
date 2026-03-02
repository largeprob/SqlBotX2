using System;
using System.Collections.Generic;
using System.Text;

namespace SqlBoTx.Net.Domain.BusinessObjectives.Events
{
   
    public record Upsert
    { 

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 业务名称
        /// </summary>
        public string? BusinessName { get; set; }

        /// <summary>
        /// 近义词（逗号分隔存储）
        /// </summary>
        public string? Synonyms { get; set; }

        /// <summary>
        /// 业务解释
        /// </summary>
        public string? Description { get; set; }

        public Upsert(int id, string? businessName, string? synonyms, string? description)
        {
            Id = id;
            BusinessName = businessName;
            Synonyms = synonyms;
            Description = description;
        }
    }

 

    public record Delete(int Id);
    
}
