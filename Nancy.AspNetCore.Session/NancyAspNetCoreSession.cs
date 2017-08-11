using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NancySession = Nancy.Session;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Nancy.Bootstrapper;
using System.Text;

namespace Nancy.AspNetCore.Session
{
    public class NancyAspNetCoreSession : NancySession.ISession
    {
        internal const string HAS_CHANGED_KEY = "__haschanged";
        internal class NancyAspNetCoreSessionIterator : IEnumerator<KeyValuePair<string, object>>
        {
            int _currentIndex = -1;

            string[] _publickeys;
            private string[] publickeys
            {
                get
                {
                    if (_session.hasChanged)
                    {
                        _publickeys = _session._httpSession.Keys.Where(k => k != HAS_CHANGED_KEY).ToArray();
                    }
                    return _publickeys;
                }
            }
            private ISession httpSession
            {
                get
                {
                    return _session._httpSession;
                }
            }
            public KeyValuePair<string, object> Current
            {
                get
                {
                    string key = publickeys.Skip(_currentIndex).FirstOrDefault();

                    if (!string.IsNullOrEmpty(key))
                    {
                        if (httpSession.TryGetValue(key, out byte[] b))
                        {
                            return new KeyValuePair<string, object>(key, AsObject(b));
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
                if (_currentIndex < publickeys.Length-1)
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
                _currentIndex = -1;
            }

            NancyAspNetCoreSession _session;
            internal NancyAspNetCoreSessionIterator(NancyAspNetCoreSession session)
            {
                _session = session;
            }
        }

        ISession _httpSession;
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

                if (_httpSession.TryGetValue(HAS_CHANGED_KEY, out byte[] b))
                {
                    return (bool)AsObject(b);
                }
                else
                { return false; }
            }
            set
            {
                byte[] bhaschanged = AsBytes(value);
                _httpSession.Set(HAS_CHANGED_KEY, bhaschanged);
            }
        }

        public int Count => _httpSession.Keys.Count() - 1;

        public bool HasChanged => hasChanged;

        internal NancyAspNetCoreSession(ISession httpSession)
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
            return new NancyAspNetCoreSessionIterator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new NancyAspNetCoreSessionIterator(this);
        }
        
        private static byte[] AsBytes(object value)
        {            
            string json = JsonConvert.SerializeObject(value, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }) ;

            byte[] bJson = UnicodeEncoding.Unicode.GetBytes(json);
            return bJson;
        }

        private static object AsObject(byte[] data)
        {
            object value = null;
            string json=UnicodeEncoding.Unicode.GetString(data);
            //string json = Convert.ToBase64String(data);
            value = JsonConvert.DeserializeObject(json, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

            return value;
        }
        
        public static void Enable(IPipelines pipelines,IHttpContextAccessor httpCtxAcs)
        {
            pipelines.BeforeRequest.AddItemToStartOfPipeline(ctx => LoadSession(ctx, httpCtxAcs.HttpContext));
            pipelines.AfterRequest.AddItemToEndOfPipeline(async ctx => await SaveSession(ctx));
        }

        private static Response LoadSession(NancyContext context, HttpContext httpCtx)
        {
            ISession session = httpCtx.Session;
            context.Request.Session = new NancyAspNetCoreSession(session);

            return null;
        }

        private static async Task SaveSession(NancyContext context)
        {
            var session = context.Request.Session as NancyAspNetCoreSession;
            session.hasChanged = false;
            await session?.SaveAsync();
        }
    }
}