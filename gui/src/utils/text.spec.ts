import { describe, it, expect } from "vitest";
import {
  base64ToBlob,
  countNewlineCharacters,
  fromBase64,
  toBase64,
} from "@/utils/text";

describe("text", function () {
  it("should correctly convert from base64 string", () => {
    const input = "dGVzdA==";
    const result = fromBase64(input);
    expect(result).toEqual("test");
  });

  it("should return undefined when converting invalid string from base64 string", () => {
    const input = "()*";
    const result = fromBase64(input);
    expect(result).toBeUndefined();
  });

  it("should correctly convert to base64 string", () => {
    const input = "test";
    const result = toBase64(input);
    expect(result).toEqual("dGVzdA==");
  });

  it("should correctly count new lines", () => {
    const input = "this\ncontains new\r\nlines";
    const result = countNewlineCharacters(input);
    expect(result).toEqual(3);
  });

  it("should correctly convert base64 string to blob", async () => {
    const input = "dGVzdA==";
    const result = base64ToBlob(input);
    expect(result.size).toEqual(4);
  });

  it("should correctly count new lines if string contains no new lines", () => {
    const input = "this string contains no new lines";
    const result = countNewlineCharacters(input);
    expect(result).toEqual(1);
  });
});
