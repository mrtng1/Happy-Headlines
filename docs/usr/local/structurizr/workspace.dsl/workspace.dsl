workspace "HappyHeadlines" "C4 model of the HappyHeadlines system" {

  model {

    // People
    pub = person "Publisher" "Writes, saves drafts, publishes."
    rdr = person "Reader"    "Reads articles, comments, subscribes."

    // System + containers
    hh = softwareSystem "HappyHeadlines" "Positive news site." {

      lb = container "Load Balancer" {
        technology "Nginx"
        description "Reverse proxy for UI and APIs."
      }

      webapp = container "WebApp" {
        technology "Blazor (WASM)"
        description "UI for publisher and reader."
      }

      draftSvc = container "DraftService" {
        technology "ASP.NET Core API"
        description "Create/Update/Get drafts."
      }

      publisherSvc = container "PublisherService" {
        technology "ASP.NET Core API"
        description "Finalise publish; profanity check; enqueue."
      }

      articleSvc = container "ArticleService" {
        technology "ASP.NET Core API"
        description "Persist/fetch articles; consume from queue."
      }

      commentSvc = container "CommentService" {
        technology "ASP.NET Core API"
        description "Create/Get comments; profanity check."
      }

      profanitySvc = container "ProfanityService" {
        technology "ASP.NET Core API"
        description "Words list / moderation."
      }

      newsletterSvc = container "NewsletterService" {
        technology "ASP.NET Core API + Quartz"
        description "Immediate + daily digests."
      }

      subsSvc = container "SubscriptionsService" {
        technology "ASP.NET Core API"
        description "Store subscribers (SQLite)."
      }

      draftDb = container "DraftDatabase" {
        technology "PostgreSQL"
        tags "Database"
        description "Drafts."
      }

      articleDb = container "ArticleDatabase" {
        technology "PostgreSQL"
        tags "Database"
        description "Articles by continent."
      }

      commentDb = container "CommentDatabase" {
        technology "PostgreSQL"
        tags "Database"
        description "Comments."
      }

      profanityDb = container "ProfanityDatabase" {
        technology "File/DB"
        tags "Database"
        description "Prohibited words."
      }

      subsDb = container "SubscriberDatabase" {
        technology "SQLite"
        tags "Database"
        description "Subscribers."
      }

      mq = container "Message Broker" {
        technology "RabbitMQ"
        description "Queues and exchanges."
      }

      articleQ = container "ArticleQueue" {
        technology "RabbitMQ"
        tags "Queue"
        description "Articles ready for publication."
      }

      subsQ = container "SubscriberQueue (optional)" {
        technology "RabbitMQ"
        tags "Queue"
        description "New subscribers broadcast."
      }

      mailhog = container "MailHog" {
        technology "SMTP sink"
        description "Captures emails in development."
      }
    }
    
    // People -> UI
    pub -> webapp "Use" "HTTP(S) via LB"
    rdr -> webapp "Use" "HTTP(S) via LB"

    // LB in front of UI+APIs
    lb -> webapp        "Proxy" "HTTP"
    lb -> draftSvc      "Proxy" "HTTP"
    lb -> publisherSvc  "Proxy" "HTTP"
    lb -> articleSvc    "Proxy" "HTTP"
    lb -> commentSvc    "Proxy" "HTTP"
    lb -> profanitySvc  "Proxy" "HTTP"
    lb -> newsletterSvc "Proxy" "HTTP"
    lb -> subsSvc       "Proxy" "HTTP"

    // Draft flow
    webapp   -> draftSvc "Create/Update/Get drafts" "HTTP/JSON"
    draftSvc -> draftDb  "CRUD" "SQL"

    // Publish flow
    webapp       -> publisherSvc "POST /publish" "HTTP/JSON"
    publisherSvc -> profanitySvc "Check content" "HTTP/JSON"
    publisherSvc -> mq           "Publish article" "AMQP"
    mq           -> articleQ     "Hold"
    articleSvc   -> articleQ     "Consume" "AMQP"
    articleSvc   -> articleDb    "Persist" "SQL"

    // Read flow
    webapp -> articleSvc "Get latest/highlight" "HTTP/JSON"

    // Comments flow
    webapp     -> commentSvc   "Create/Get comments" "HTTP/JSON"
    commentSvc -> profanitySvc "Check comment" "HTTP/JSON"
    commentSvc -> commentDb    "Persist" "SQL"

    // Subscriptions + Newsletter
    webapp        -> subsSvc      "POST /api/Subscriptions" "HTTP/JSON"
    subsSvc       -> subsDb       "Persist subscriber" "SQLite"
    subsSvc       -> mq           "Publish new subscriber (optional)" "AMQP"
    mq            -> subsQ        "Hold"
    newsletterSvc -> subsDb       "Read subscribers" "SQLite"
    newsletterSvc -> articleSvc   "Fetch recent articles" "HTTP/JSON"
    newsletterSvc -> mailhog      "Send emails" "SMTP"

    // Profanity data
    profanitySvc  -> profanityDb  "Load/Manage words" "I/O"
  }

  views {

    systemContext hh SystemContext {
      include *
      autoLayout lr
    }

    container hh ContainerView {
      include *
      autoLayout lr
    }

    container hh PublishPipeline {
      autoLayout lr
      include lb
      include webapp
      include publisherSvc
      include profanitySvc
      include mq
      include articleQ
      include articleSvc
      include articleDb
      include pub
    }

    container hh ReaderAndComments {
      autoLayout lr
      include lb
      include webapp
      include articleSvc
      include commentSvc
      include profanitySvc
      include articleDb
      include commentDb
      include rdr
    }

    container hh Newsletter {
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
      element "Person" {
        shape person
        background #0B5F6D
        color #ffffff
      }
      element "Container" {
        shape roundedbox
        background #177E89
        color #ffffff
      }
      element "Database" {
        shape cylinder
        background #323031
        color #ffffff
      }
      element "Queue" {
        shape pipe
        background #A23B72
        color #ffffff
      }
      relationship "Relationship" {
        routing orthogonal
        color #666666
      }
    }
  }
}