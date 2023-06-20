#if Win32

using log4net;
using NetFwTypeLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


// ref.
// sample
// https://stackoverflow.com/questions/15409790/adding-an-application-firewall-rule-to-both-private-and-public-networks-via-win7
//

namespace HCVK.HCVKSLibrary
{
	public class FirewallManager
	{
		private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


		private INetFwRule _iNetFwRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
		private INetFwPolicy2 _iNetFwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));


		public FirewallManager()
		{

		}

		~FirewallManager()
		{

		}


		public INetFwRule iNetFwRule
		{
			get { return _iNetFwRule; }
		}

		private bool CheckExistRule()
		{
			return (_iNetFwPolicy2.Rules.OfType<INetFwRule>().Where(x => x.Name == _iNetFwRule.Name).FirstOrDefault() != null) ? true : false;
		}

		public void AddRule(bool bIsForceUpdate = false)
		{
			if (string.IsNullOrEmpty(_iNetFwRule.Name))
				throw new ArgumentNullException(_iNetFwRule.ToString());

			try
			{
				if (bIsForceUpdate)
					RemoveRule();


				if (!CheckExistRule())
					_iNetFwPolicy2.Rules.Add(_iNetFwRule);
			}
			catch (Exception ex)
			{
				_logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
			}
		}

		public void RemoveRule()
		{
			try
			{
				if (CheckExistRule())
					_iNetFwPolicy2.Rules.Remove(_iNetFwRule.Name);
			}
			catch (Exception ex)
			{
				_logger.Error(string.Format("Exception[0x{0:X8}] : {1}", ex.HResult, ex.ToString()));
			}
		}
	}
}

#endif
