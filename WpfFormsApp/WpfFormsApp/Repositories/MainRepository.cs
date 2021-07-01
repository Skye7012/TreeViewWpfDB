using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfFormsApp
{
    class MainRepository
    {
        /// <summary>
        /// объединяет в себе все репозитории
        /// </summary>
        CE_Context ce_context;
        public TgroupRepository Tgroups { get; set; }
        public TrelationRepository Trelations { get; set; }
        public TpropertyRepository Tproperties { get; set; }
        public MainRepository(CE_Context context)
        {
            ce_context = context;
            Tgroups = new TgroupRepository(context);
            Trelations = new TrelationRepository(context);
            Tproperties = new TpropertyRepository(context);
        }
        public void Disconnect()
        {
            ce_context.Disconnect();
        }
    }
}
