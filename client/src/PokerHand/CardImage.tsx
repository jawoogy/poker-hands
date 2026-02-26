import deckSvgUrl from "../assets/English_pattern_playing_cards_deck.svg?url";

// The deck SVG is a 13-column × 4-row grid.
// Each cell is 390 × 570 px; the card rect sits 30 px in from each edge.
const CELL_W = 390;
const CELL_H = 570;
const CARD_PAD = 30;
const CARD_W = 360;
const CARD_H = 540;

const SUIT_ROW: Record<string, number> = { S: 0, H: 1, D: 2, C: 3 };
const FACE_COL: Record<string, number> = {
  A: 0, "2": 1, "3": 2, "4": 3, "5": 4, "6": 5,
  "7": 6, "8": 7, "9": 8, "10": 9, J: 10, Q: 11, K: 12,
};

interface Props {
  /** Card code matching the backend format, e.g. "AS", "10H", "KC", "2D" */
  card: string;
  /** Display width in px; height scales proportionally. Default 80. */
  width?: number;
}

export function CardImage({ card, width = 80 }: Props) {
  const suit = card.slice(-1).toUpperCase();
  const face = card.slice(0, -1).toUpperCase();

  const row = SUIT_ROW[suit];
  const col = FACE_COL[face];

  if (row === undefined || col === undefined) return null;

  const vx = col * CELL_W + CARD_PAD;
  const vy = row * CELL_H + CARD_PAD;
  const height = Math.round((width / CARD_W) * CARD_H);

  return (
    <svg
      viewBox={`${vx} ${vy} ${CARD_W} ${CARD_H}`}
      width={width}
      height={height}
      style={{ display: "block" }}
    >
      <image href={deckSvgUrl} width={5100} height={2310} />
    </svg>
  );
}
