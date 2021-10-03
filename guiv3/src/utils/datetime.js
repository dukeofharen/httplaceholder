import { dateTimeFormat } from "@/constants/technical";
import dayjs from "dayjs";

export function formatDateTime(input) {
  return dayjs(input).format(dateTimeFormat);
}

export function formatFromNow(input) {
  return dayjs(input).fromNow();
}

export function getDuration(fromInput, toInput) {
  const from = dayjs(fromInput);
  const to = dayjs(toInput);
  return to.diff(from, "millisecond");
}
