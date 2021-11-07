import { dateTimeFormat } from "@/constants/technical";
import dayjs from "dayjs";

export function formatDateTime(input) {
  if (!input) {
    return;
  }

  return dayjs(input).format(dateTimeFormat);
}

export function formatFromNow(input) {
  if (!input) {
    return;
  }

  return dayjs(input).fromNow();
}

export function getDuration(fromInput, toInput) {
  const from = dayjs(fromInput);
  const to = dayjs(toInput);
  return to.diff(from, "millisecond");
}
