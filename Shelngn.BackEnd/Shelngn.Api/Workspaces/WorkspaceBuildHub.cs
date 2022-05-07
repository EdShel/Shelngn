﻿using Microsoft.AspNetCore.SignalR;
using Shelngn.Services.GameProjects.Build;

namespace Shelngn.Api.Workspaces
{
    public partial class WorkspaceHub
    {
        [HubMethodName("build")]
        public async Task BuildProject()
        {
            var builder = Resolve<IGameProjectBuilder>();

            await DispatchToWorkspaceAsync(new
            {
                type = "wokspace/build/begin"
            });

            string workspaceId = this.WorkspaceId;
            BuildResult result = await builder.BuildProjectAsync(workspaceId);

            if (result.IsSuccess)
            {
                await DispatchToWorkspaceAsync(new
                {
                    type = "wokspace/build/finish"
                });
            }
            else
            {
                await DispatchToWorkspaceAsync(new
                {
                    type = "wokspace/build/failed",
                    error = result.Error
                });
            }
        }
    }

}
