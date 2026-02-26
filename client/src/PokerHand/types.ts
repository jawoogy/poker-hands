export interface PokerHand {
    player: string;
    cards: string[]; // 2-9,10,A,K,Q,J with HSDC 2H, AS, etc..
}

export interface HandRequest {
    hand1: PokerHand;
    hand2: PokerHand;
}

export interface HandResponse {
    winner: string;
    hand: string;
    rank: number;
    handEval: string;
}

export type ActiveSlot = {
    hi: number; // hand index 0 or 1
    ci: number; // card index 0-4
} | null;

