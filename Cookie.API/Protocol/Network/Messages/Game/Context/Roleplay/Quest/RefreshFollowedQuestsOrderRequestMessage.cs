//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.42000
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cookie.API.Protocol.Network.Messages.Game.Context.Roleplay.Quest
{
    using System.Collections.Generic;
    using Cookie.API.Protocol.Network.Messages;
    using Cookie.API.Protocol.Network.Types;
    using Cookie.API.IO;
    
    
    public class RefreshFollowedQuestsOrderRequestMessage : NetworkMessage
    {
        
        public const uint ProtocolId = 6722;
        
        public override uint MessageID
        {
            get
            {
                return ProtocolId;
            }
        }
        
        private List<System.UInt16> m_quests;
        
        public virtual List<System.UInt16> Quests
        {
            get
            {
                return m_quests;
            }
            set
            {
                m_quests = value;
            }
        }
        
        public RefreshFollowedQuestsOrderRequestMessage(List<System.UInt16> quests)
        {
            m_quests = quests;
        }
        
        public RefreshFollowedQuestsOrderRequestMessage()
        {
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteShort(((short)(m_quests.Count)));
            int questsIndex;
            for (questsIndex = 0; (questsIndex < m_quests.Count); questsIndex = (questsIndex + 1))
            {
                writer.WriteVarUhShort(m_quests[questsIndex]);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            int questsCount = reader.ReadUShort();
            int questsIndex;
            m_quests = new System.Collections.Generic.List<ushort>();
            for (questsIndex = 0; (questsIndex < questsCount); questsIndex = (questsIndex + 1))
            {
                m_quests.Add(reader.ReadVarUhShort());
            }
        }
    }
}