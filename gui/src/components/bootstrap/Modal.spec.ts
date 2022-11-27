import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { mount } from "@vue/test-utils";
import Modal from "@/components/bootstrap/Modal.vue";
import type { Modal as BootstrapModal } from "bootstrap";
import * as bootstrapUtil from "@/utils/bootstrap";

describe("Modal", () => {
  let bootstrapModal: BootstrapModal;

  beforeEach(() => {
    bootstrapModal = {
      show: () => {
        console.log("show");
      },
      hide: () => {
        console.log("hide");
      },
    } as BootstrapModal;
    vi.spyOn(bootstrapModal, "show");
    vi.spyOn(bootstrapModal, "hide");
    vi.spyOn(bootstrapUtil, "getOrCreateInstance").mockImplementation(() => {
      return bootstrapModal;
    });
  });

  afterEach(() => {
    vi.clearAllMocks();
  });

  it("renders properly", () => {
    const wrapper = mount(Modal, {
      props: {
        title: "Test title",
      },
    });
    expect(wrapper.html()).contains("Test title");
  });

  it("should open modal on rendering", () => {
    const wrapper = mount(Modal, {
      props: {
        title: "Test title",
        showModal: true,
      },
    });
    expect(bootstrapModal.show).toHaveBeenCalledOnce();
    expect(bootstrapModal.hide).toHaveBeenCalledTimes(0);
    expect(bootstrapUtil.getOrCreateInstance).toHaveBeenCalledWith(
      wrapper.vm.modal
    );
  });

  it("should close modal on rendering", () => {
    const wrapper = mount(Modal, {
      props: {
        title: "Test title",
        showModal: false,
      },
    });
    expect(bootstrapModal.show).toHaveBeenCalledTimes(0);
    expect(bootstrapModal.hide).toHaveBeenCalledOnce();
    expect(bootstrapUtil.getOrCreateInstance).toHaveBeenCalledWith(
      wrapper.vm.modal
    );
  });

  it("should close open modal after showModal property changed", async () => {
    const wrapper = mount(Modal, {
      props: {
        title: "Test title",
        showModal: false,
      },
    });
    expect(bootstrapModal.show).toHaveBeenCalledTimes(0);
    expect(bootstrapModal.hide).toHaveBeenCalledOnce();
    await wrapper.setProps({
      showModal: true,
    });
    expect(bootstrapModal.show).toHaveBeenCalledOnce();
  });

  it("should execute 'yes' function and hide modal when Yes is clicked", () => {
    let yesClicked = false;
    const wrapper = mount(Modal, {
      props: {
        title: "Test title",
        showModal: true,
        yesClickFunction: () => (yesClicked = true),
        yesText: "Aye",
      },
    });
    wrapper.find(".yes-button").trigger("click");
    expect(yesClicked).toEqual(true);
    expect(bootstrapModal.hide).toHaveBeenCalledOnce();
    expect(wrapper.html()).contains("Aye");
  });

  it("should execute 'no' function and hide modal when No is clicked", () => {
    let noClicked = false;
    const wrapper = mount(Modal, {
      props: {
        title: "Test title",
        showModal: true,
        noClickFunction: () => (noClicked = true),
        noText: "Naw",
      },
    });
    wrapper.find(".no-button").trigger("click");
    expect(noClicked).toEqual(true);
    expect(bootstrapModal.hide).toHaveBeenCalledOnce();
    expect(wrapper.html()).contains("Naw");
  });
});
