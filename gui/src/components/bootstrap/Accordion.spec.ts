import { describe, it, expect } from "vitest";
import Accordion from "@/components/bootstrap/Accordion.vue";
import { mount } from "@vue/test-utils";

describe("Accordion", () => {
  it("renders properly", () => {
    const wrapper = mount(Accordion);
    expect(wrapper.html()).contains('id="accordion"');
  });
});
