using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace protean.Epicor
{
    public class SalesRep
    {
        #region Private Members

        private string _repGroupId = "";
        private string _repGroupDescription = "";
        private string _repRegion = "";
        private bool _salesRepIsInActive = true;
        private bool _isPopulated = false;        

        #endregion

        #region Properties

        public string SalesRepCode { get; set; } = "";

        /// <summary>
        /// Rep group Id
        /// </summary>
        public string RepGroupId
        {
            get
            {
                if (!_isPopulated)
                {
                    Populate();
                }
                return _repGroupId;
            }
            set
            {
                _repGroupId = value;
            }
        }

        /// <summary>
        /// Rep Group Description
        /// </summary>
        public string RepGroupDescription
        {
            get
            {
                if (!_isPopulated)
                {
                    Populate();
                }
                return _repGroupDescription;
            }
            set
            {
                _repGroupDescription = value;
            }
        }

        /// <summary>
        /// Is sales rep active or inactive.
        /// Epicor has it in the negative (InActive) rather than Active
        /// </summary>
        public bool SalesRepIsInactive
        {
            get
            {
                if (!_isPopulated)
                {
                    Populate();
                }
                return _salesRepIsInActive;
            }
            set
            {
                _salesRepIsInActive = value;
            }
        }

        /// <summary>
        /// Region of the sales rep.  In the case that this is a sales manager, this is 
        /// the region that will be used for thier account.
        /// </summary>
        public string RepRegion
        {
            get
            {
                if (!_isPopulated)
                {
                    Populate();
                }
                return _repRegion;
            }
            set
            {
                _repRegion = value;
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Populate the class
        /// </summary>
        private void Populate()
        {
            dynamic workforce = EAL.Workforce.GetWorkforceBySalesRepCode(SalesRepCode);
            if (workforce != null && workforce.value != null)
            {
                _repGroupId = workforce.value[0].SalesRep_ShortChar01;
                _repGroupDescription = workforce.value[0].UDCodes_CodeDesc;
                _repRegion = workforce.value[0].SalesRep_ShortChar02;
                _salesRepIsInActive = Convert.ToBoolean(workforce.value[0].SalesRep_InActive);
            }
            _isPopulated = true;
        }

        #endregion

    }
}