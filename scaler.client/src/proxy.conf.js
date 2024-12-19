const PROXY_CONFIG = [
  {
    context: [
      "/api",
      "/swagger",
      "/connect",
      "/oauth",
      "/.well-known"
    ],
    target: "https://localhost:7085",
    secure: false
  }
]

module.exports = PROXY_CONFIG;
