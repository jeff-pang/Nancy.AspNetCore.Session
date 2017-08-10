using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NancySession = Nancy.Session;
using Microsoft.AspNetCore.Http;
using Http = Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Nancy.Bootstrapper;
using System.Text;

namespace Nancy.AspNetCore.Session
{
    public class NancyAspNetCoreSession : NancySession.ISession
    {
        internal class NancyAspNetCoreSessionIterator : IEnumerator<KeyValuePair<string, object>>
        {
            int _currentIndex = 0;

            public KeyValuePair<string, object> Current
            {
                get
                {
                    string k = _session.Keys.Skip(_currentIndex).FirstOrDefault();
                    if (!string.IsNullOrEmpty(k))
                    {
                        if (_session.TryGetValue(k, out byte[] b))
                        {
                            return new KeyValuePair<string, object>(k, AsObject(b));
                        }
                    }
                    throw new IndexOutOfRangeException();
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_currentIndex < _session.Keys.Count() - 1)
                {
                    _currentIndex++;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void Reset()
            {
                _currentIndex = 0;
            }

            Http.ISession _session;
            internal NancyAspNetCoreSessionIterator(Http.ISession httpSession)
            {
                _session = httpSession;
            }
        }

        Http.ISession _httpSession;
        public object this[string key]
        {
            get
            {
                if(_httpSession.TryGetValue(key,out byte[] b))
                {
                    return AsObject(b);
                }

                return null;

            }
            set
            {
                byte[] b = AsBytes(value);
                _httpSession.Set(key, b);
                hasChanged = true;
            }
        }

        private bool hasChanged {
            get
            {

                if (_httpSession.TryGetValue("_haschanged", out byte[] b))
                {
                    return (bool)AsObject(b);
                }
                else
                { return false; }
            }
            set
            {
                byte[] bhaschanged = AsBytes(value);
                _httpSession.Set("_haschanged", bhaschanged);
            }
        }

        public int Count => _httpSession.Keys.Count();

        public bool HasChanged => hasChanged;

        internal NancyAspNetCoreSession(Http.ISession httpSession)
        {
            _httpSession = httpSession;
            hasChanged = false;
        }

        public void Delete(string key)
        {
            if (_httpSession.Keys.Contains(key))
            {
                _httpSession.Remove(key);
                hasChanged = true;
            }
        }

        public void DeleteAll()
        {
            string[] keys = _httpSession.Keys.ToArray();

            if (keys.Length>0)
            {
                foreach (string k in keys)
                {
                    _httpSession.Remove(k);
                }
            }
        }

        public async Task SaveAsync()
        {
            await _httpSession.CommitAsync();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new NancyAspNetCoreSessionIterator(_httpSession);
        }
        
        private static byte[] AsBytes(object value)
        {            
            string json = JsonConvert.SerializeObject(value, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }) ;

            byte[] bJson = UnicodeEncoding.Unicode.GetBytes(json);
            return bJson;
        }

        private static object AsObject(byte[] data)
        {
            object value = null;
            string json=UnicodeEncoding.Unicode.GetString(data);
            //string json = Convert.ToBase64String(data);
            value = JsonConvert.DeserializeObject(json, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });

            return value;
        }
        
        public static void Enable(IPipelines pipelines)
        {
            pipelines.BeforeRequest.AddItemToStartOfPipeline(ctx => LoadSession(ctx));
            pipelines.AfterRequest.AddItemToEndOfPipeline(async ctx => await SaveSession(ctx));
        }

        private static Response LoadSession(NancyContext context)
        {
            ISession session = InternalHttpContextAccessorSingleton.Instance.HttpContextAccessor.HttpContext.Session;
            context.Request.Session = new NancyAspNetCoreSession(session);

            return null;
        }

        private static async Task SaveSession(NancyContext context)
        {
            var session = context.Request.Session as NancyAspNetCoreSession;

            await session?.SaveAsync();
        }
    }
}