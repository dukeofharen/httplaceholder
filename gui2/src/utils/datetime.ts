import { dateTimeFormat } from "@/constants/technical";
import dayjs from "dayjs";

export function formatDateTime(input: string) {
  if (!input) {
    return;
  }

  return dayjs(input).format(dateTimeFormat);
}

export function formatFromNow(input: string) {
  if (!input) {
    return;
  }

  return dayjs(input).fromNow();
}

export function getDuration(
  fromInput: string,
  toInput: string
): number | undefined {
  if (!fromInput || !toInput) {
    return;
  }

  const from = dayjs(fromInput);
  const to = dayjs(toInput);
  return to.diff(from, "millisecond");
}
