using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bol.App.Core.Services
{
	public class PermissionService : IPermissionService
	{
		public async Task<bool> CheckStoragePermission()
		{
			var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
			if (status == PermissionStatus.Denied) status = await Permissions.RequestAsync<Permissions.StorageRead>();
			if (status == PermissionStatus.Denied) return false;
			else return true;
		}
		public async Task<bool> CheckCameraPermission()
		{
			var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
			if (status == PermissionStatus.Denied) status = await Permissions.RequestAsync<Permissions.Camera>();
			if (status == PermissionStatus.Denied) return false;
			else return true;
		}
		public async Task<bool> CheckSpeechPermission()
		{
			var status = await Permissions.CheckStatusAsync<Permissions.Speech>();
			if (status == PermissionStatus.Denied) status = await Permissions.RequestAsync<Permissions.Speech>();
			if (status == PermissionStatus.Denied) return false;
			else return true;
		}
	}
}
