import {useState} from "react"
import { CardImage } from "./CardImage"
import { SUITS, RANKS, CARD_WIDTH } from "./constants"; 

interface CardPickerProps {
    usedCards: string[]; // These ar all the cards that we have known and loved.  Or just simply picked.. 
    onSelect: (card: string) => void;
    onClose: ()=> void;
}

export function CardPicker({usedCards, onSelect, onClose}: CardPickerProps) {
    const [suit, setSuit] = useState<string | null>(null);

      return (
    <div style={{
        background: "#0f2d1a",
        border: "2px solid #c9a84c",
        borderRadius: 12,
        padding: "1.5rem",
        minWidth: 320,
        }}>
      {suit === null ? (
        // Stage 1: show suits
        <div style={{ display: "flex", gap: 12 }}>
          {SUITS.map(s => (
            <button key={s.code} type="button"
              onClick={() => setSuit(s.code)}
              style={{ fontSize: CARD_WIDTH, color: s.color }}>
              {s.symbol}
            </button>
          ))}
        </div>
      ) : (
        // Stage 2: show ranks for chosen suit
        <>
          <button type="button" onClick={() => setSuit(null)}
            style={{ background: "none", color: "#c9a84c", border: "none", fontSize: "1rem", marginBottom: "0.75rem", cursor: "pointer", padding: 0 }}>
            ← Back
            </button>
          <div style={{ display: "flex", flexWrap: "wrap", gap: 4 }}>
            {RANKS.map(rank => {
              const card = `${rank}${suit}`;
              const used = usedCards.includes(card);
              return (
                <button key={rank} type="button" disabled={used}
                  onClick={() => onSelect(card)}
                  style={{ opacity: used ? 0.25 : 1, background: "none", border: "none", padding: 2 }}>
                  <CardImage card={card} width={CARD_WIDTH} />
                </button>
              );
            })}
          </div>
        </>
      )}
      <button type="button" onClick={onClose}
        style={{ marginTop: "1rem", background: "#4a1a1a", color: "#f5f0e0", border: "1px solid #c9a84c44", borderRadius: 6, padding: "4px 14px", display: "block" }}>
        Cancel
        </button>
    </div>
  );
}