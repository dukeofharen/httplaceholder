<template>
  <div class="col-auto col-md-3 col-xl-2 px-sm-2 px-0 bg-dark">
    <div
      class="
        d-flex
        flex-column
        align-items-center align-items-sm-start
        px-3
        pt-2
        text-white
        min-vh-100
      "
    >
      <a
        href="/"
        class="
          d-flex
          align-items-center
          pb-3
          mb-md-0
          me-md-auto
          text-white text-decoration-none
        "
      >
        <span class="fs-5 d-none d-sm-inline logo"
          ><img src="@/assets/logo-white_small.png"
        /></span>
      </a>
      <ul
        class="
          nav nav-pills
          flex-column
          mb-sm-auto mb-0
          align-items-center align-items-sm-start
        "
        id="menu"
      >
        <li class="nav-item">
          <a href="#" class="nav-link align-middle px-0" title="Requests">
            <i class="fs-4 bi-eye"></i>
            <span class="ms-3 d-none d-sm-inline">Requests</span>
          </a>
        </li>
        <li class="nav-item">
          <a href="#" class="nav-link align-middle px-0" title="Stubs">
            <i class="fs-4 bi-code-slash"></i>
            <span class="ms-3 d-none d-sm-inline">Stubs</span>
          </a>
        </li>
        <li class="nav-item">
          <a href="#" class="nav-link align-middle px-0" title="Add stubs">
            <i class="fs-4 bi-plus"></i>
            <span class="ms-3 d-none d-sm-inline">Add stubs</span>
          </a>
        </li>
        <li class="nav-item">
          <a href="#" class="nav-link align-middle px-0" title="Upload stubs">
            <i class="fs-4 bi-upload"></i>
            <span class="ms-3 d-none d-sm-inline">Upload stubs</span>
          </a>
        </li>
        <li class="nav-item">
          <a href="#" class="nav-link align-middle px-0" title="Download stubs">
            <i class="fs-4 bi-download"></i>
            <span class="ms-3 d-none d-sm-inline">Download stubs</span>
          </a>
        </li>
        <li class="nav-item">
          <a href="#" class="nav-link align-middle px-0" title="Settings">
            <i class="fs-4 bi-wrench"></i>
            <span class="ms-3 d-none d-sm-inline">Settings</span>
          </a>
        </li>
        <li class="nav-item">
          <a href="#" class="nav-link align-middle px-0" title="Log out">
            <i class="fs-4 bi-box-arrow-left"></i>
            <span class="ms-3 d-none d-sm-inline">Log out</span>
          </a>
        </li>
      </ul>
    </div>
  </div>
</template>

<script>
export default {
  name: "Sidebar",
};
</script>

<style lang="scss" scoped>
.logo img {
  max-width: 100%;
}
</style>
