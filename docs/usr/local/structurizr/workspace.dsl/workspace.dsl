workspace "HappyHeadlines" "C4 model of the HappyHeadlines system" {

  model {

    // People
    person pub "Publisher" "Writes, saves drafts, publishes."
    person rdr "Reader"    "Reads articles, comments, subscribes."

    // System + containers
    softwareSystem hh "HappyHeadlines" "Positive news site." {

      container lb "Load Balancer" {
        technology "Nginx"
        description "Reverse proxy for UI and APIs."
      }

      container webapp "WebApp" {
        technology "Blazor (WASM)"
        description "UI for publisher and reader."
      }

      container draftSvc "DraftService" {
        technology "ASP.NET Core API"
        description "Create/Update/Get drafts."
      }

      container publisherSvc "PublisherService" {
        technology "ASP.NET Core API"
        description "Finalise publish; profanity check; enqueue."
      }

      container articleSvc "ArticleService" {
        technology "ASP.NET Core API"
        description "Persist/fetch articles; consume from queue."
      }

      container commentSvc "CommentService" {
        technology "ASP.NET Core API"
        description "Create/Get comments; profanity check."
      }

      container profanitySvc "ProfanityService" {
        technology "ASP.NET Core API"
        description "Words list / moderation."
      }

      container newsletterSvc "NewsletterService" {
        technology "ASP.NET Core API + Quartz"
        description "Immediate + daily digests."
      }

      container subsSvc "SubscriptionsService" {
        technology "ASP.NET Core API"
        description "Store subscribers (SQLite)."
      }

      container draftDb "DraftDatabase" {
        technology "PostgreSQL"
        tags "Database"
        description "Drafts."
      }

      container articleDb "ArticleDatabase" {
        technology "PostgreSQL"
        tags "Database"
        description "Articles by continent."
      }

      container commentDb "CommentDatabase" {
        technology "PostgreSQL"
        tags "Database"
        description "Comments."
      }

      container profanityDb "ProfanityDatabase" {
        technology "File/DB"
        tags "Database"
        description "Prohibited words."
      }

      container subsDb "SubscriberDatabase" {
        technology "SQLite"
        tags "Database"
        description "Subscribers."
      }

      container mq "Message Broker" {
        technology "RabbitMQ"
        description "Queues and exchanges."
      }

      container articleQ "ArticleQueue" {
        technology "RabbitMQ"
        tags "Queue"
        description "Articles ready for publication."
      }

      container subsQ "SubscriberQueue (optional)" {
        technology "RabbitMQ"
        tags "Queue"
        description "New subscribers broadcast."
      }

      container mailhog "MailHog" {
        technology "SMTP sink"
        description "Captures emails in development."
      }
    }
    
    // People -> UI
    pub -> hh.webapp "Use" "HTTP(S) via LB"
    rdr -> hh.webapp "Use" "HTTP(S) via LB"

    // LB in front of UI+APIs
    hh.lb -> hh.webapp        "Proxy" "HTTP"
    hh.lb -> hh.draftSvc      "Proxy" "HTTP"
    hh.lb -> hh.publisherSvc  "Proxy" "HTTP"
    hh.lb -> hh.articleSvc    "Proxy" "HTTP"
    hh.lb -> hh.commentSvc    "Proxy" "HTTP"
    hh.lb -> hh.profanitySvc  "Proxy" "HTTP"
    hh.lb -> hh.newsletterSvc "Proxy" "HTTP"
    hh.lb -> hh.subsSvc       "Proxy" "HTTP"

    // Draft flow
    hh.webapp   -> hh.draftSvc "Create/Update/Get drafts" "HTTP/JSON"
    hh.draftSvc -> hh.draftDb  "CRUD" "SQL"

    // Publish flow
    hh.webapp       -> hh.publisherSvc "POST /publish" "HTTP/JSON"
    hh.publisherSvc -> hh.profanitySvc "Check content" "HTTP/JSON"
    hh.publisherSvc -> hh.mq           "Publish article" "AMQP"
    hh.mq           -> hh.articleQ     "Hold"
    hh.articleSvc   -> hh.articleQ     "Consume" "AMQP"
    hh.articleSvc   -> hh.articleDb    "Persist" "SQL"

    // Read flow
    hh.webapp -> hh.articleSvc "Get latest/highlight" "HTTP/JSON"

    // Comments flow
    hh.webapp     -> hh.commentSvc   "Create/Get comments" "HTTP/JSON"
    hh.commentSvc -> hh.profanitySvc "Check comment" "HTTP/JSON"
    hh.commentSvc -> hh.commentDb    "Persist" "SQL"

    // Subscriptions + Newsletter
    hh.webapp        -> hh.subsSvc      "POST /api/Subscriptions" "HTTP/JSON"
    hh.subsSvc       -> hh.subsDb       "Persist subscriber" "SQLite"
    hh.subsSvc       -> hh.mq           "Publish new subscriber (optional)" "AMQP"
    hh.mq            -> hh.subsQ        "Hold"
    hh.newsletterSvc -> hh.subsDb       "Read subscribers" "SQLite"
    hh.newsletterSvc -> hh.articleSvc   "Fetch recent articles" "HTTP/JSON"
    hh.newsletterSvc -> hh.mailhog      "Send emails" "SMTP"

    // Profanity data
    hh.profanitySvc  -> hh.profanityDb  "Load/Manage words" "I/O"
  }

  views {

    systemContext hhCtx "System Context" {
      include *
      autoLayout lr
    }

    container hh "Container View" {
      include *
      include element "pub"
      include element "rdr"
      autoLayout lr
    }

    container hh "Publish Pipeline" {
      autoLayout lr
      include lb
      include webapp
      include publisherSvc
      include profanitySvc
      include mq
      include articleQ
      include articleSvc
      include articleDb
      include element "pub"
    }

    container hh "Reader & Comments" {
      autoLayout lr
      include lb
      include webapp
      include articleSvc
      include commentSvc
      include profanitySvc
      include articleDb
      include commentDb
      include element "rdr"
    }

    container hh "Newsletter" {
      autoLayout lr
      include lb
      include webapp
      include subsSvc
      include subsDb
      include newsletterSvc
      include articleSvc
      include mailhog
      include mq
      include subsQ
    }

    styles {
      element "Person"    { shape person;     background #0B5F6D; color #ffffff; }
      element "Container" { shape roundedbox; background #177E89; color #ffffff; }
      element "Database"  { shape cylinder;   background #323031; color #ffffff; }
      element "Queue"     { shape pipe;       background #A23B72; color #ffffff; }
      relationship        { routing orthogonal; color #666666; }
    }
  }
}
