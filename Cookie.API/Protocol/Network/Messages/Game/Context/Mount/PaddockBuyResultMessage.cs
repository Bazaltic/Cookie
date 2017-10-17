﻿using Cookie.API.Utils.IO;

namespace Cookie.API.Protocol.Network.Messages.Game.Context.Mount
{
    public class PaddockBuyResultMessage : NetworkMessage
    {
        public const ushort ProtocolId = 6516;

        public PaddockBuyResultMessage(double paddockId, bool bought, ulong realPrice)
        {
            PaddockId = paddockId;
            Bought = bought;
            RealPrice = realPrice;
        }

        public PaddockBuyResultMessage()
        {
        }

        public override ushort MessageID => ProtocolId;
        public double PaddockId { get; set; }
        public bool Bought { get; set; }
        public ulong RealPrice { get; set; }

        public override void Serialize(IDataWriter writer)
        {
            writer.WriteDouble(PaddockId);
            writer.WriteBoolean(Bought);
            writer.WriteVarUhLong(RealPrice);
        }

        public override void Deserialize(IDataReader reader)
        {
            PaddockId = reader.ReadDouble();
            Bought = reader.ReadBoolean();
            RealPrice = reader.ReadVarUhLong();
        }
    }
}