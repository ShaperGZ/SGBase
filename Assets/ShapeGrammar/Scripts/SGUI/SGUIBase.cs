using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;

namespace SGGUI
{
    public interface IGrammarOperator
    {
        void SetGrammar(Grammar g);
        void UnSetGrammar();
    }
}