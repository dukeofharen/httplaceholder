import { dateTimeFormat } from "@/constants/technical";
import dayjs from "dayjs";

export function formatDateTime(input) {
  return dayjs(input).format(dateTimeFormat);
}

export function formatFromNow(input) {
  return dayjs(input).fromNow();
}
