/*Add responses table*/
CREATE TABLE IF NOT EXISTS `responses`
(
    `id`             BIGINT(20) NOT NULL AUTO_INCREMENT,
    `status_code`    INT        NOT NULL,
    `headers`        LONGTEXT   NOT NULL,
    `body`           LONGTEXT   NOT NULL,
    `body_is_binary` TINYINT(1) NOT NULL,
    PRIMARY KEY (`id`),
    FOREIGN KEY (id) REFERENCES requests (id) ON DELETE CASCADE
) ENGINE = INNODB
  AUTO_INCREMENT = 1
  DEFAULT CHARSET = UTF8MB4;
