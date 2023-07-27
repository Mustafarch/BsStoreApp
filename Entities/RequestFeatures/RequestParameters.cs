using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.RequestFeatures
{
    public abstract class RequestParameters
    {
		const int maxPageSize = 50;

		//Auto-implemented property
        public int PageNumber { get; set; }
		private int _pageSize;

		//Full-property
		public int PageSize
		{
			get { return _pageSize; }
			set { _pageSize = value > maxPageSize ? maxPageSize : value; } //değer 50 den büyük geliyorsa bu durumda maxPageSize dönececk ama 50 den küçük ise mesela 10 ise value buna destek var ve 10 olabilir. 
		}

		public String? OrderBy { get; set; }   //Sıralama işlemi için oluşturduk.

		public String? Fields { get; set; }	   //Veri şekillendirme (Shape) için oluşturduk.
	}
}
