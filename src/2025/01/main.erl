#!/usr/bin/env escript

main(Args) ->
    %% Default to "input.txt" if no file is passed via CLI
    Filename = case Args of
        [File] -> File;
        [] -> "input.txt"
    end,
    
    %% Read and parse
    Lines = read_lines(Filename),
    ParsedData = parse(Lines),
    
    %% Output the results
    io:format("Part 1: ~p~n", [part_1(ParsedData)]),
    io:format("Part 2: ~p~n", [part_2(ParsedData)]).

read_lines(Filename) ->  
  {ok, Binary} = file:read_file(Filename),
  string:lexemes(binary_to_list(Binary), "\n").

%% parse L100, R5 etc. strings
parse_line([$L | NumberStr])->
  {left, list_to_integer(NumberStr)};

parse_line([$R | NumberStr])->
  {right, list_to_integer(NumberStr)}.

parse(Lines) ->
    [parse_line(Line) || Line <- Lines].

move({left, Dist}, {Pos, ZeroCount}) ->
  Next = wrap(Pos - Dist),
  Zc = case Next of
      0 -> ZeroCount + 1;
      _ -> ZeroCount
      end,
      
  {Next, Zc};
  
move({right, Dist}, {Pos, ZeroCount}) ->
  Next = wrap(Pos+Dist),
  
  Zc = case Next of
      0 -> ZeroCount + 1;
      _ -> ZeroCount
      end,

  {Next, Zc}.

wrap(X)->
  (X rem 100 + 100) rem 100.
  
part_1(Data) ->
  {_, Result} = lists:foldl(fun move/2, {50, 0}, Data),
  Result.

part_2(_) ->
  -1.
