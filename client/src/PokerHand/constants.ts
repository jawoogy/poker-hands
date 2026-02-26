import type { PokerHand } from "./types";

export const SUITS = [
  { code: "H", symbol: "♥", color: "red" },
  { code: "D", symbol: "♦", color: "red" },
  { code: "C", symbol: "♣", color: "black" },
  { code: "S", symbol: "♠", color: "black" },
];

export const RANKS = ["2","3","4","5","6","7","8","9","10","J","Q","K","A"];

export const FULL_DECK = RANKS.flatMap(r => SUITS.map(s => `${r}${s.code}`));

export const RANK_NAMES: Record<number, string> = {
  1: "High Card",
  2: "One Pair",
  3: "Two Pair",
  4: "Three of a Kind",
  5: "Straight",
  6: "Flush",
  7: "Full House",
  8: "Four of a Kind",
  9: "Straight Flush",
};

export const CARD_WIDTH = 100;
export const CARD_HEIGHT = Math.round(CARD_WIDTH * 1.5);

export const EMPTY_HAND=(player:string): PokerHand => ({
    player, 
    cards: ["","","","",""],
});
