namespace Commons

module Monads =
    type Result<'TSuccess> =
        | Success of 'TSuccess
        | Fail of string