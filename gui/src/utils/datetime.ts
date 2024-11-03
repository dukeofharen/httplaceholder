import { dateTimeFormat } from "@/constants";
import dayjs from "dayjs";
import { translate } from "@/utils/translate";

export function formatDateTime(input: string): string | undefined {
  if (!input) {
    return;
  }

  return dayjs(input).format(dateTimeFormat);
}

export function formatFromNow(input: string): string | undefined {
  if (!input) {
    return;
  }

  return dayjs(input).locale(translate("dayJsLocale")).fromNow();
}

export function getDuration(
  fromInput: string,
  toInput: string,
): number | undefined {
  if (!fromInput || !toInput) {
    return;
  }

  const from = dayjs(fromInput);
  const to = dayjs(toInput);
  return to.diff(from, "millisecond");
}
