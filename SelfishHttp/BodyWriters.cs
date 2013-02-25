using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace SelfishHttp
{
    public class BodyWriters : IBodyWriter
    {
        private readonly List<TypeWriter> _bodyWriters = new List<TypeWriter>();

        public void RegisterBodyWriter<T>(Action<T, Stream> writeBody, ResponseEncoding encoding)
        {
            _bodyWriters.Insert(0, new TypeWriter { Type = typeof(T), Writer = (o, stream) => writeBody((T)o, stream), ResponseEncoding = encoding });
        }

        public class TypeWriter
        {
            public Type Type;
            public Action<object, Stream> Writer;
            public ResponseEncoding ResponseEncoding;
        }

        public static IBodyWriter DefaultBodyWriter()
        {
            var writers = new BodyWriters();
            writers.RegisterBodyWriter<Stream>((stream, outputStream) => stream.CopyTo(outputStream), ResponseEncoding.PlainText);
            writers.RegisterBodyWriter<string>((str, outputStream) =>
                                                  {
                                                      using (var streamWriter = new StreamWriter(outputStream))
                                                      {
                                                          streamWriter.Write(str);
                                                      }
                                                  }, ResponseEncoding.PlainText);
            
            writers.RegisterBodyWriter<string>((str, outputStream)=>
                                                {
                                                    var encoding = new ASCIIEncoding();
                                                    var data = encoding.GetBytes(str);
                                                    using (var hgs = new GZipStream(outputStream, CompressionMode.Compress))
                                                    {
                                                        hgs.Write(data, 0, data.Length);
                                                    }
                                                
                                                }, ResponseEncoding.GZip);

            return writers;
        }

        public void WriteBody(object o, Stream stream, ResponseEncoding encoding)
        {
            var writeBody = FindWriterForType(o.GetType(), encoding);

            if (writeBody != null)
            {
                writeBody(o, stream);
            } else
            {
                throw new ApplicationException(string.Format("could not convert body of type {0}", o.GetType()));
            }
        }

        private Action<object, Stream> FindWriterForType(Type type, ResponseEncoding responseEncoding)
        {
            TypeWriter writer = _bodyWriters.FirstOrDefault(typeWriter => typeWriter.Type.IsAssignableFrom(type)
                && typeWriter.ResponseEncoding == responseEncoding);
            return writer != null? writer.Writer: null;
        }
    }
}