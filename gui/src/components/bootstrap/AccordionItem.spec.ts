import { describe, it, expect } from "vitest";
import { mount } from "@vue/test-utils";
import AccordionItem from "@/components/bootstrap/AccordionItem.vue";
import SlideUpDown from "vue3-slide-up-down";

describe("Accordion", () => {
  it("renders properly", () => {
    const wrapper = mount(AccordionItem, {
      global: {
        components: {
          SlideUpDown,
        },
      },
    });
    expect(wrapper.html()).contains("accordion-button");
  });

  it("should trigger 'buttonClicked' when 'opened' is not null and button is clicked", () => {
    const wrapper = mount(AccordionItem, {
      global: {
        components: {
          SlideUpDown,
        },
      },
      props: { opened: false },
    });
    wrapper.find(".accordion-button").trigger("click");
    expect(wrapper.emitted()).toHaveProperty("buttonClicked");
  });

  it("should trigger 'opened' when 'opened' is null and button is clicked", () => {
    const wrapper = mount(AccordionItem, {
      global: {
        components: {
          SlideUpDown,
        },
      },
      props: { opened: null },
    });
    wrapper.find(".accordion-button").trigger("click");
    expect(wrapper.emitted()).toHaveProperty("opened");
  });

  it("should trigger 'closed' when 'opened' is null and button is clicked", () => {
    const wrapper = mount(AccordionItem, {
      global: {
        components: {
          SlideUpDown,
        },
      },
      props: { opened: null },
    });
    const button = wrapper.find(".accordion-button");
    button.trigger("click");
    expect(wrapper.emitted()).toHaveProperty("opened");
    button.trigger("click");
    expect(wrapper.emitted()).toHaveProperty("closed");
  });
});
