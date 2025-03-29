﻿using System.Text;
using Microsoft.AspNetCore.Http;

namespace MyAspNetMvcProject.Tests.utils
{
    public class FakeSession : ISession
    {
        private readonly Dictionary<string, byte[]> _sessionStorage = new();

        public bool IsAvailable => true;
        public string Id => Guid.NewGuid().ToString();

        public IEnumerable<string> Keys => _sessionStorage.Keys;

        public void Clear() => _sessionStorage.Clear();

        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public void Remove(string key) => _sessionStorage.Remove(key);

        public void Set(string key, byte[] value) => _sessionStorage[key] = value;

        public bool TryGetValue(string key, out byte[] value) => _sessionStorage.TryGetValue(key, out value);

        public void SetString(string key, string value) => Set(key, Encoding.UTF8.GetBytes(value));

        public string? GetString(string key) => TryGetValue(key, out var value) ? Encoding.UTF8.GetString(value) : null;
    }
}
