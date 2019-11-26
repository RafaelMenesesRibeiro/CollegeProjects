using dida_clients.helpers;
using dida_contracts.domain_objects;
using dida_contracts.exceptions;

namespace dida_clients.domain_objects.xu_liskov_impl
{
    public class XuLiskovForegroundClient : ForegroundClient
    {
        public XuLiskovForegroundClient(XuLiskovBackgroundClient backgroundClient) : base(backgroundClient) { }

        public override void Write(DIDATuple tuple)
        {
            ((XuLiskovBackgroundClient)backgroundClient).ConsumeRequest(tuple, EXuLiskovOperation.Write);
        }

        public override void Read(DIDATuple tuple)
        {
            ((XuLiskovBackgroundClient)backgroundClient).ConsumeRequest(tuple, EXuLiskovOperation.Read);
        }

        public override void Take(DIDATuple tuple)
        {
            ((XuLiskovBackgroundClient)backgroundClient).ConsumeRequest(tuple, EXuLiskovOperation.TakeOne);
        }
    }
}
