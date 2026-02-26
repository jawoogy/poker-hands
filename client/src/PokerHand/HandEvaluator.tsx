import React, { useState } from "react";
import type { ActiveSlot, HandRequest, HandResponse, PokerHand } from "./types";
import { CardImage } from "./CardImage";
import { CardPicker } from "./CardPicker";
import { FULL_DECK, RANK_NAMES, CARD_HEIGHT, CARD_WIDTH, EMPTY_HAND } from "./constants";




export function HandEvaluator() {
    const [hand1, setHand1] = useState<PokerHand>(EMPTY_HAND("Player 1"));
    const [hand2, setHand2] = useState<PokerHand>(EMPTY_HAND("Player 2"));
    const [result, setResult] = useState<HandResponse | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [activeSlot, setActiveSlot] = useState<ActiveSlot>(null);
    const hands = [hand1, hand2];
    const setHands = [setHand1, setHand2];


const handleSubmit = async (e: React.SubmitEvent) => {
    e.preventDefault();
    setError(null);
    setResult(null);

    try {
        const body: HandRequest = {hand1, hand2 };

        const res = await fetch("/api/hands/eval", {
            method: "POST",
            headers: {"Content-Type": "application/json" },
            body: JSON.stringify(body),
        });

        if (!res.ok) {
            setError(`Error ${res.status}: ${await res.text()}`);
            return;
        }

        setResult(await res.json());
    } catch (err) {
        setError(err instanceof Error ? err.message : "Network error");
    }
};

const usedCards = (): string[]=> {
  const active = activeSlot ? hands[activeSlot.hi].cards[activeSlot.ci]: "";
  return [...hand1.cards, ...hand2.cards].filter(c=> c != "" && c !== active);
};

const selectedCard = (card: string) => {
  if (!activeSlot) return;
  const {hi, ci} = activeSlot;
  const cards = [...hands[hi].cards];
  cards[ci] = card;
  setHands[hi]({...hands[hi], cards});
  setActiveSlot(null);
};

const resetHand = (hi: number) => {
  setHands[hi]({ ...hands[hi], cards: ["","","","",""] });
};

const randomizeHand = (hi: number) => {
  const otherCards = hands[1 - hi].cards.filter(c => c !== "");
  const available = FULL_DECK.filter(c => !otherCards.includes(c));
  const shuffled = [...available].sort(() => Math.random() - 0.5);
  setHands[hi]({ ...hands[hi], cards: shuffled.slice(0, 5) });
};

const isReady = hand1.cards.every(c=> c !== "") && hand2.cards.every(c=> c !== "");

return (
   <form onSubmit={handleSubmit}>
    <h1 style={{textAlign: "center", color: "#c9a84c", fontSize: "2.4rem", letterSpacing: "0.12em", textShadow: "0 2px 6px rgba(0,0,0,0.6)", marginBottom: "1.5rem",}}>Poker Hand Evaluator</h1>
      {[{ hand: hand1, setHand: setHand1 }, { hand: hand2, setHand: setHand2 }].map(
        ({ hand, setHand }, hi) => (
          <div key={hi} style={{background: "#0f2d1a", borderRadius: 12, padding: "1rem 1.5rem", marginBottom: "1.5rem", border:"1px solid #c9a84c44"}}>
            <input
              style={{background: "#1a4a2e", color:"#f5f0e0", border: "1px solid #c9a84c", borderRadius: 6, padding: "6px 12px", fontSize:"1rem", marginBottom: "0.75rem", display: "block"}}
              value={hand.player}
              onChange={(e) => setHand({ ...hand, player: e.target.value })}
              placeholder={`Player ${hi + 1} name`}
            />
            <div style={{ display: "flex", gap: 8, marginBottom: "0.75rem" }}>
              <button type="button" onClick={() => randomizeHand(hi)}
                style={{ background: "#2d6a4f", color: "#f5f0e0", border: "1px solid #c9a84c44", borderRadius: 6, padding: "4px 14px" }}>
                Randomize
              </button>
              <button type="button" onClick={() => resetHand(hi)}
                style={{ background: "#4a1a1a", color: "#f5f0e0", border: "1px solid #c9a84c44", borderRadius: 6, padding: "4px 14px" }}>
                Reset
              </button>
            </div>
            <div style={{ display: "flex", flexWrap: "wrap", gap: 8 }}>  {/* ← outside map */}
              {hand.cards.map((card, j) => (
                <div key={j} style={{ display: "inline-flex", flexDirection: "column", alignItems: "center", gap: 4 }}>
                  <button type="button"
                    onClick={() => setActiveSlot(
                      activeSlot?.hi == hi && activeSlot?.ci == j ? null : {hi, ci: j}
                    )}>
                    {card.length >= 2
                      ? <CardImage card={card} width={CARD_WIDTH} />
                      : <div style={{ width: CARD_WIDTH, height: CARD_HEIGHT, border: "2px dashed #aaa", borderRadius: 6 }} />
                    }
                  </button>
                </div>
              ))}
</div>
          </div>
        )
      )}

      <button type="submit" 
      disabled={!isReady} 
      style={{
        background: isReady ? "#c9a84c" : "#5a5a3a",
        color: isReady ? "#1a1a1a" : "#999",
        border: "none",
        borderRadius: 8,
        padding: "10px 32px",
        fontSize: "1.1rem",
        fontWeight: "bold",
        letterSpacing: "0.08em",
        cursor: isReady ? "pointer" : "not-allowed",
        marginTop: "1rem",
      }}>Evaluate</button>

      {error && <p style={{ color: "red" }}>{error}</p>}
      {result && (
        <div onClick={() => setResult(null)} style={{
          position: "fixed",
          top: 0, left: 0, right: 0, bottom: 0,
          background: "rgba(0,0,0,0.6)",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          zIndex: 1000,
          cursor: "pointer",
        }}>
          <div style={{
            background: "#0f2d1a",
            border: "2px solid #c9a84c",
            borderRadius: 12,
            padding: "2rem",
            textAlign: "center",
            minWidth: 360,
          }}>
            <div style={{ fontSize: "1.8rem", color: "#c9a84c", fontWeight: "bold", marginBottom: "0.5rem" }}>
              {result.winner === "Tie" ? "It's a Tie!" : `${result.winner} Wins! `}
            </div>
            <div style={{ fontSize: "1.1rem", color: "#f5f0e0", letterSpacing: "0.08em", marginBottom: "1rem" }}>
              {result.handEval === "" ? RANK_NAMES[result.rank] : result.handEval}
            </div>
            {result.winner !== "Tie" && (
              <div style={{ display: "flex", flexWrap: "wrap", justifyContent: "center", gap: 8 }}>
                {result.hand.split(", ").map(card => (
                  <CardImage key={card} card={card} width={CARD_WIDTH} />
                ))}
              </div>
            )}
            {result.winner === "Tie" && (
              <div style={{ color: "#aaa", fontSize: "0.95rem" }}>{result.hand}</div>
            )}
            <div style={{ marginTop: "1rem", color: "#c9a84c88", fontSize: "0.85rem" }}>
              click anywhere to dismiss
            </div>
          </div>
        </div>
      )}
      {activeSlot !== null && (
        <div style={{
          position: "fixed",
          top: 0, left: 0, right: 0, bottom: 0,
          background: "rgba(0,0,0,0.5)",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          zIndex: 1000,
        }}>
          <div onClick={()=> setActiveSlot(null)} style={{position:"fixed",}} >
            <div onClick={e => e.stopPropagation()}>  {/* prevent backdrop click from closing when clicking picker */}
          <CardPicker
            usedCards={usedCards()}
            onSelect={selectedCard}
            onClose={() => setActiveSlot(null)}
          />
          </div>
          </div>
      </div>
      )}
      
    </form>
  );

}