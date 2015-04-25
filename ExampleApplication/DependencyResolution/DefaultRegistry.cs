// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Configuration;
using Microsoft.Azure.Documents.Client;
using StackExchange.Redis;
using StructureMap.Web.Pipeline;

namespace ExampleApplication.DependencyResolution {
    using StructureMap.Configuration.DSL;
    using StructureMap.Graph;
	
    public class DefaultRegistry : Registry {
        #region Constructors and Destructors

        public DefaultRegistry() {
            Scan(
                scan => {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
					scan.With(new ControllerConvention());
                });
            var documentEndpoint = new Uri(ConfigurationManager.AppSettings["documentEndpoint"]);
            var documentKey = ConfigurationManager.AppSettings["documentKey"];
            For<DocumentClient>()
                .Use(() => new DocumentClient(documentEndpoint, documentKey, null, null))
                .LifecycleIs<HttpContextLifecycle>();

            var redisEndpoint = ConfigurationManager.AppSettings["redisEndpoint"];
            var redisKey = ConfigurationManager.AppSettings["redisKey"];
            For<ConnectionMultiplexer>()
                .Use(() => ConnectionMultiplexer.Connect(redisEndpoint + ",abortConnect=false,ssl=true,password=" + redisKey, null))
                .LifecycleIs<HttpContextLifecycle>();
        }

        #endregion
    }
}