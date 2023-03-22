using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    /// <summary>
    /// Base type for all of the data types that load via the content pipeline.
    /// </summary>
    public abstract class ContentObject
    {
        /// <summary>
        /// The name of the content pipeline asset that contained this object.
        /// </summary>
        private string assetName;

        /// <summary>
        /// The name of the content pipeline asset that contained this object.
        /// </summary>
        [ContentSerializerIgnore]
        public string AssetName
        {
            get { return assetName; }
            set { assetName = value; }
        }
    }
}
