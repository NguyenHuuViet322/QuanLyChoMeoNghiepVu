﻿using ESD.Infrastructure.HttpClientAccessors.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ESD.Infrastructure.HttpClientAccessors.Implementations
{
    public class BaseHttpClientFactory : IBaseHttpClientFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public BaseHttpClientFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IBaseHttpClient Create()
        {
            return _serviceProvider.GetRequiredService<IBaseHttpClient>();
        }
    }
}
