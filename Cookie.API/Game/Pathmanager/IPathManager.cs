﻿using Cookie.API.Core;

namespace Cookie.API.Game.Pathmanager
{
    public interface IPathManager
    {
        bool Launched { get; set; }
        ICharacter Character { get; set; }
        void Start(string trajet);
        void Stop();
        void DoAction();
    }
}