﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Models;
using System.Collections.Generic;

namespace JuCheap.Core.WebApi
{
    public class Config
    {
		// scopes define the API resources in your system
		public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API")
            };
        }

        // client want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",
	                // 没有交互性用户，使用 clientid/secret 实现认证。
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
	                // 用于认证的密码
                    ClientSecrets = 
                    {
                        new Secret("secret".Sha256())
                    },
	                // 客户端有权访问的范围（Scopes）
                    AllowedScopes = { "api1" }
                },
	            // resource owner password grant client
	            new Client
	            {
		            ClientId = "ro.client",
		            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

		            ClientSecrets =
		            {
			            new Secret("secret".Sha256())
		            },
		            AllowedScopes = { "api1" }
	            }
			};
        }

	    public static List<TestUser> GetUsers()
	    {
		    return new List<TestUser>
		    {
			    new TestUser
			    {
				    SubjectId = "1",
				    Username = "alice",
				    Password = "password"
			    },
			    new TestUser
			    {
				    SubjectId = "2",
				    Username = "bob",
				    Password = "password"
			    }
		    };
	    }
	}
}