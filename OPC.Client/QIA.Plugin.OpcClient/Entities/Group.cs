using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QIA.Plugin.OpcClient.Entities
{
    public class Group : BaseEntity
    {
        public string Name { get; set; }
        public int? AppSettingsId { get; set; }
        public virtual AppSettings AppSettings { get; set; }
    }
}
