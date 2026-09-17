import { describe, it, expect } from 'vitest';
import { validateAlbum } from './validators';
import { validateDate, validateIPV6 } from "./validators";

// test the validateDate function
describe('validateDate', () => {
    it('should return true for a valid date', () => {
        expect(validateDate('2021-01-01')).toBe(true);
    });

    it('should return false for an invalid date', () => {
        expect(validateDate('2021-13-01')).toBe(false);
    });

    it('should return false for an empty date', () => {
        expect(validateDate('')).toBe(false);
    });
}); 

