using System;
using System.Net.Sockets;

namespace ReverseProxy.NET6.Lib
{
    public class CopyStream
    {
        private static readonly EasLog logger = IEasLog.CreateLogger("CopyStream");
        private readonly NetworkStream m_destStream;
        private readonly TcpClient m_destClient;
        private readonly Action m_onClose;
        private readonly NetworkStream m_sourceStream;
        private readonly TcpClient m_sourceClient;
        public CopyStream(TcpClient source, TcpClient dest, Action onClose)
        {
            try
            {
                m_destClient = dest;
                m_onClose = onClose;
                m_sourceClient = source;
                m_sourceStream = m_sourceClient.GetStream();
                m_destStream = m_destClient.GetStream();
                var sourceBytes = new byte[1000000];
                m_sourceStream.BeginRead(sourceBytes, 0, sourceBytes.Length, EndSourceRead, sourceBytes);
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Copy stream initialization failed");
            }
        }

        private void EndSourceRead(IAsyncResult ar)
        {
            try
            {
                var sourceBytes = (byte[]?)ar.AsyncState;
                var read = m_sourceStream.EndRead(ar);
                if (read > 0)
                {
                    if (m_destClient.Connected)
                        m_destStream.BeginWrite(sourceBytes, 0, read, EndDestWrite, null);
                    if (m_sourceClient.Connected)
                        m_sourceStream.BeginRead(sourceBytes, 0, sourceBytes.Length, EndSourceRead, sourceBytes);
                }
                else
                {
                    //m_sourceStream.Close();
                    //m_sourceClient.Close();
                    m_onClose();
                }
            }
            catch (Exception ex)
            {
                var source = m_sourceClient.Client?.RemoteEndPoint?.ToString();
                var dest = m_destClient.Client?.RemoteEndPoint?.ToString();
                //m_sourceStream.Close();
                //m_sourceClient.Close();
                m_onClose();
                logger.Exception(ex, source ?? "-", dest ?? "-", "While reading from source an exception occured. Closing connection");
            }
        }

        private void EndDestWrite(IAsyncResult ar)
        {
            try
            {
                m_destStream.EndWrite(ar);
            }
            catch (Exception ex)
            {
                var source = m_sourceClient.Client?.RemoteEndPoint?.ToString();
                var dest = m_destClient.Client?.RemoteEndPoint?.ToString();
                //m_sourceStream.Close();
                //m_sourceClient.Close();
                m_onClose();
                logger.Exception(ex, source ?? "-", dest ?? "-", "While writing to destination. Exception occured closing connection");

            }
        }
    }
}