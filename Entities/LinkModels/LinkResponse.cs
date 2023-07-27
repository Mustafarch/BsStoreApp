using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.LinkModels
{
    public class LinkResponse
    {
        public bool HasLinks { get; set; }    //Link var mı yok mu.
        public List<Entity> ShapedEntities { get; set; }  //Liste içinde şekillendirilmiş varlıklar tutulacak.
        public LinkCollectionWrapper<Entity> LinkedEntities { get; set; }

        public LinkResponse()
        {
            ShapedEntities = new List<Entity>();                    //Bu ikiside referans tipli ifadeler. Bunların ctor da başlatılması gerek.
                                                                    //LinkResponse Serilazer edileceği için ctor kullanılmalı.
            LinkedEntities = new LinkCollectionWrapper<Entity>();
        }
    }
}
