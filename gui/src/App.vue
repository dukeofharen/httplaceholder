<template>
  <div id="app">
    <nav class="navbar navbar-expand-lg navbar-light bg-light">
      <a class="navbar-brand" href="#">HttPlaceholder</a>
      <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
        <span class="navbar-toggler-icon"></span>
      </button>

      <div class="collapse navbar-collapse" id="navbarSupportedContent">
        <ul class="navbar-nav mr-auto">
          <li class="nav-item">
            <router-link to="/requests" class="nav-link">Requests</router-link>
          </li>
          <li class="nav-item dropdown">
            <a class="nav-link dropdown-toggle" href="#" id="stubsDropdown" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
              Stubs
            </a>
            <div class="dropdown-menu" aria-labelledby="stubsDropdown">
              <router-link to="/stubs" class="dropdown-item">Stubs</router-link>
              <router-link to="/addStub" class="dropdown-item">Add stub</router-link>
              <router-link to="/downloadStubs" class="dropdown-item">Download stubs</router-link>
            </div>
          </li>
        </ul>
      </div>
    </nav>
    <div class="container application">
      <router-view></router-view>
    </div>
  </div>
</template>

<style>
#app {
  font-family: "Avenir", Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}
#nav {
  padding: 30px;
}

#nav a {
  font-weight: bold;
  color: #2c3e50;
}

#nav a.router-link-exact-active {
  color: #42b983;
}
</style>
